﻿using System.Linq;
using System.Text.RegularExpressions;
using scbot.bot;
using scbot.services.compareengine;
using scbot.services.persistence;
using scbot.services.zendesk;
using scbot.utils;

namespace scbot.processors.zendesk
{
    public class ZendeskTicketTracker : IMessageProcessor
    {
        private const string c_PersistenceKey = "tracked-zd-tickets";

        private readonly ICommandParser m_CommandParser;
        private readonly IListPersistenceApi<Tracked<ZendeskTicket>> m_Persistence;
        private readonly IZendeskTicketApi m_ZendeskApi;
        private static readonly Regex s_ZendeskIdRegex = new Regex(@"^ZD#(?<id>\d{5})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly ZendeskTicketCompareEngine m_ZendeskTicketCompareEngine;

        public ZendeskTicketTracker(ICommandParser commandParser, IKeyValueStore persistence, IZendeskTicketApi zendeskApi)
        {
            m_CommandParser = commandParser;
            m_Persistence = new ListPersistenceApi<Tracked<ZendeskTicket>>(persistence);
            m_ZendeskApi = zendeskApi;
            m_ZendeskTicketCompareEngine = new ZendeskTicketCompareEngine(m_Persistence);
        }

        public MessageResult ProcessTimerTick()
        {
            var trackedTickets = m_Persistence.ReadList(c_PersistenceKey);

            var comparison = trackedTickets.Select(x =>
                new Update<ZendeskTicket>(x.Channel, x.Value, m_ZendeskApi.FromId(x.Value.Id).Result)
            ).Where(x => x.NewValue.IsNotDefault());

            var responses = m_ZendeskTicketCompareEngine.CompareTicketStates(comparison);

            return new MessageResult(responses.ToList());
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack, toUntrack;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_ZendeskIdRegex.IsMatch(toTrack))
            {
                var ticket = m_ZendeskApi.FromId(toTrack.Substring(3)).Result;
                m_Persistence.AddToList(c_PersistenceKey, new Tracked<ZendeskTicket>(ticket, message.Channel));
                return new MessageResult(new[] {Response.ToMessage(message, FormatNowTrackingMessage(toTrack))});
            }
            if (m_CommandParser.TryGetCommand(message, "untrack", out toUntrack) && s_ZendeskIdRegex.IsMatch(toUntrack))
            {
                var idToUntrack = toUntrack.Substring(3);
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Value.Id == idToUntrack);
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