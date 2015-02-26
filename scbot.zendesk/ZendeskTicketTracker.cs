using System.Linq;
using System.Text.RegularExpressions;
using scbot.core.bot;
using scbot.core.compareengine;
using scbot.core.persistence;
using scbot.core.utils;
using scbot.zendesk.services;

namespace scbot.zendesk
{
    public class ZendeskTicketTracker : IMessageProcessor
    {
        private const string c_PersistenceKey = "tracked-zd-tickets";

        private readonly ICommandParser m_CommandParser;
        private readonly IListPersistenceApi<Tracked<ZendeskTicket>> m_Persistence;
        private readonly IZendeskTicketApi m_ZendeskApi;
        private static readonly Regex s_ZendeskIdRegex = new Regex(@"^ZD#(?<id>\d{5})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal readonly CompareEngine<ZendeskTicket> m_ZendeskTicketCompareEngine; // internal for tests -- should be injected?

        public ZendeskTicketTracker(ICommandParser commandParser, IKeyValueStore persistence, IZendeskTicketApi zendeskApi)
        {
            m_CommandParser = commandParser;
            m_Persistence = new ListPersistenceApi<Tracked<ZendeskTicket>>(persistence);
            m_ZendeskApi = zendeskApi;
            m_ZendeskTicketCompareEngine = new CompareEngine<ZendeskTicket>(
                x => string.Format("<https://redgatesupport.zendesk.com/agent/tickets/{0}|ZD#{0}> ({1}) updated:", x.Id, x.Description),
                new[]
                {
                    new PropertyComparer<ZendeskTicket>(x => x.OldValue.Comments.Count < x.NewValue.Comments.Count, FormatCommentsAdded),
                    new PropertyComparer<ZendeskTicket>(x => x.OldValue.Status != x.NewValue.Status, FormatStatusChanged), 
                    new PropertyComparer<ZendeskTicket>(x => x.OldValue.Description != x.NewValue.Description, FormatDescriptionChanged), 
                });
        }

        private static Response FormatCommentsAdded(Update<ZendeskTicket> x)
        {
            var diff = (x.NewValue.Comments.Count - x.OldValue.Comments.Count);
            if (diff == 1)
            {
                var addedComment = x.NewValue.Comments.Last();
                return new Response(addedComment.Author + " added a comment", null, addedComment.Avatar);
            }
            return new Response(string.Format("{0} comments added", diff), null);
        }

        private static Response FormatStatusChanged(Update<ZendeskTicket> x)
        {
            return new Response(string.Format("`{0}` \u2192 `{1}`", x.OldValue.Status, x.NewValue.Status), null);
        }

        private static Response FormatDescriptionChanged(Update<ZendeskTicket> x)
        {
            return new Response("description updated", null);
        }

        public MessageResult ProcessTimerTick()
        {
            var trackedTickets = m_Persistence.ReadList(c_PersistenceKey);

            var comparison = trackedTickets.Select(x =>
                new Update<ZendeskTicket>(x.Channel, x.Value, m_ZendeskApi.FromId(x.Value.Id).Result)
            ).Where(x => x.NewValue.IsNotDefault());

            var results = m_ZendeskTicketCompareEngine.Compare(comparison).ToList();

            foreach (var result in results)
            {
                var id = result.NewValue.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Value.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new Tracked<ZendeskTicket>(result.NewValue, result.Response.Channel));
            }

            return new MessageResult(results.Select(x => x.Response).ToList());
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