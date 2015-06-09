using System;
using System.Linq;
using System.Text.RegularExpressions;
using scbot.core.bot;
using scbot.core.compareengine;
using scbot.core.persistence;
using scbot.core.utils;
using scbot.zendesk.services;
using scbot.core.meta;

namespace scbot.zendesk
{
    public class ZendeskTicketTracker : IMessageProcessor
    {
        public static IFeature Create(ICommandParser commandParser, IKeyValueStore persistence)
        {
            var zendeskApi = new ErrorCatchingZendeskTicketApi(
                new ZendeskTicketApi(new CachedZendeskApi(new Time(), ReconnectingZendeskApi.CreateAsync(
                    async () => await ZendeskApi.CreateAsync(Configuration.RedgateId), new Time()).Result)));

            var zendeskTicketTracker = new ZendeskTicketTracker(commandParser, persistence, zendeskApi);
            var zendeskTicketProcessor = new ZendeskTicketProcessor(zendeskApi);
            return new BasicFeature("zdtracker", "track comments added to zendesk tickets", "use `track ZD#12345` to start tracking a zendesk ticket in the current channel",
                                        new CompositeMessageProcessor(zendeskTicketProcessor, zendeskTicketTracker));
        }

        private readonly ICommandParser m_CommandParser;
        private readonly IListPersistenceApi<Tracked<ZendeskTicket>> m_Persistence;
        private readonly IZendeskTicketApi m_ZendeskApi;
        private static readonly Regex s_ZendeskIdRegex = new Regex(@"(?:ZD#(?<id>\d{5})|\<https\:\/\/redgatesupport.zendesk.com\/agent\/tickets\/(?<id>\d{5})\>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        internal readonly CompareEngine<ZendeskTicket> m_ZendeskTicketCompareEngine; // internal for tests -- should be injected?

        public ZendeskTicketTracker(ICommandParser commandParser, IKeyValueStore persistence, IZendeskTicketApi zendeskApi)
        {
            m_CommandParser = commandParser;
            m_Persistence = new ListPersistenceApi<Tracked<ZendeskTicket>>(persistence, "tracked-zd-tickets");
            m_ZendeskApi = zendeskApi;
            m_ZendeskTicketCompareEngine = new CompareEngine<ZendeskTicket>(
                x => string.Format("<https://redgatesupport.zendesk.com/agent/tickets/{0}|ZD#{0}> ({1}) updated:", x.Id, x.Description),
                new[]
                {
                    new PropertyComparer<ZendeskTicket>(x => x.OldValue.Comments.Count < x.NewValue.Comments.Count, FormatCommentsAdded),
                    new PropertyComparer<ZendeskTicket>(ClosedOrOpened, FormatStatusChanged), 
                    new PropertyComparer<ZendeskTicket>(x => x.OldValue.Description != x.NewValue.Description, FormatDescriptionChanged), 
                });
        }

        private bool ClosedOrOpened(Update<ZendeskTicket> x)
        {
            return x.OldValue.Status != x.NewValue.Status && 
            (x.OldValue.Status == "closed" || x.NewValue.Status == "closed");
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
            var trackedTickets = m_Persistence.ReadList();

            var comparison = trackedTickets.Select(x =>
                new Update<ZendeskTicket>(x.Channel, x.Value, m_ZendeskApi.FromId(x.Value.Id).Result)
            ).Where(x => x.NewValue.IsNotDefault());

            var results = m_ZendeskTicketCompareEngine.Compare(comparison).ToList();

            foreach (var result in results)
            {
                var id = result.NewValue.Id;
                m_Persistence.RemoveFromList(x => x.Value.Id == id);
                m_Persistence.AddToList(new Tracked<ZendeskTicket>(StripCommentText(result.NewValue), result.Response.Channel));
            }

            return new MessageResult(results.Select(x => x.Response).ToList());
        }

        private ZendeskTicket StripCommentText(ZendeskTicket newValue)
        {
            return new ZendeskTicket(newValue.Id, newValue.Description, newValue.Status, 
                newValue.Comments.Select(x => new ZendeskTicket.Comment("<text>", x.Author, x.Avatar)).ToList());
        }

        public MessageResult ProcessMessage(Message message)
        {
            string toTrack, toUntrack;
            Match idMatch;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_ZendeskIdRegex.TryMatch(toTrack, out idMatch))
            {
                var ticket = m_ZendeskApi.FromId(idMatch.Group("id")).Result;
                m_Persistence.AddToList(new Tracked<ZendeskTicket>(ticket, message.Channel));
                return Response.ToMessage(message, FormatNowTrackingMessage(idMatch.Group("id")));
            }
            if (m_CommandParser.TryGetCommand(message, "untrack", out toUntrack) && s_ZendeskIdRegex.TryMatch(toUntrack, out idMatch))
            {
                m_Persistence.RemoveFromList(x => x.Value.Id == idMatch.Group("id"));
                return Response.ToMessage(message, FormatNowNotTrackingMessage(idMatch.Group("id")));
            }
            return MessageResult.Empty;
        }

        private string FormatNowNotTrackingMessage(string id)
        {
            return string.Format(@"No longer tracking ZD#{0}.", id);
        }

        private static string FormatNowTrackingMessage(string id)
        {
            return string.Format("Now tracking ZD#{0}. To stop tracking, use `untrack ZD#{0}`", id);
        }
    }
}