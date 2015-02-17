using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using scbot.services;

namespace scbot.processors
{
    public class ZendeskTicketTracker : IMessageProcessor
    {
        private const string c_PersistenceKey = "tracked-zd-tickets";

        private struct TrackedTicket
        {
            public readonly ZendeskTicket Ticket;
            public readonly string Channel;

            public TrackedTicket(ZendeskTicket ticket, string channel)
            {
                Ticket = ticket;
                Channel = channel;
            }
        }

        private class TrackedTicketComparison
        {
            public readonly string Channel;
            public readonly string Id;
            public readonly ZendeskTicket OldValue;
            public readonly ZendeskTicket NewValue;

            public TrackedTicketComparison(string channel, string id, ZendeskTicket oldValue, ZendeskTicket newValue)
            {
                Channel = channel;
                Id = id;
                OldValue = oldValue;
                NewValue = newValue;
            }
        }

        private readonly ICommandParser m_CommandParser;
        private readonly IListPersistenceApi<TrackedTicket> m_Persistence;
        private readonly IZendeskApi m_ZendeskApi;
        private static readonly Regex s_ZendeskIdRegex = new Regex(@"^ZD#(?<id>\d{5})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public ZendeskTicketTracker(ICommandParser commandParser, IKeyValueStore persistence, IZendeskApi zendeskApi)
        {
            m_CommandParser = commandParser;
            m_Persistence = new ListPersistenceApi<TrackedTicket>(persistence);
            m_ZendeskApi = zendeskApi;
        }

        public MessageResult ProcessTimerTick()
        {
            var trackedTickets = m_Persistence.ReadList(c_PersistenceKey);

            var comparison = trackedTickets.Select(x => 
                new TrackedTicketComparison(x.Channel, x.Ticket.Id, x.Ticket, m_ZendeskApi.FromId(x.Ticket.Id).Result)
            ).Where(x => x.NewValue.IsNotDefault());

            var responses = CompareTicketStates(comparison);

            return new MessageResult(responses.ToList());
        }

        private IEnumerable<Response> CompareTicketStates(IEnumerable<TrackedTicketComparison> comparison)
        {
            var different = comparison.Where(x =>
                x.OldValue.Status != x.NewValue.Status ||
                x.OldValue.CommentCount != x.NewValue.CommentCount ||
                x.OldValue.Description != x.NewValue.Description
                ).ToList();

            var responses = different.Select(x => new Response(
                string.Format("Ticket <https://redgatesupport.zendesk.com/agent/tickets/{0}|ZD#{0}> was updated",
                    x.Id), x.Channel));

            foreach (var diff in different)
            {
                var id = diff.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Ticket.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new TrackedTicket(diff.NewValue, diff.Channel));
                Console.WriteLine("Diff: \nold: {0}\nnew:{1}", Json.Encode(diff.OldValue), Json.Encode(diff.NewValue));
            }
            return responses;
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack, toUntrack;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_ZendeskIdRegex.IsMatch(toTrack))
            {
                var ticket = m_ZendeskApi.FromId(toTrack.Substring(3)).Result;
                m_Persistence.AddToList(c_PersistenceKey, new TrackedTicket(ticket, message.Channel));
                return new MessageResult(new[] {Response.ToMessage(message, FormatNowTrackingMessage(toTrack))});
            }
            if (m_CommandParser.TryGetCommand(message, "untrack", out toUntrack) && s_ZendeskIdRegex.IsMatch(toUntrack))
            {
                var idToUntrack = toUntrack.Substring(3);
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Ticket.Id == idToUntrack);
                return new MessageResult(new[] {Response.ToMessage(message, FormatNowNotTrackingMessage(toUntrack))});
            }
            return MessageResult.Empty;
        }

        private string FormatNowNotTrackingMessage(string toUntrack)
        {
            return string.Format(@"No longer tracking {0}.", toUntrack);
        }

        private static string FormatNowTrackingMessage(string toTrack)
        {
            return string.Format("Now tracking {0}. To stop tracking, use `scbot untrack {0}`", toTrack);
        }
    }
}