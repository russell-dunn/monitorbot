using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using scbot.bot;
using scbot.services.compareengine;
using scbot.services.persistence;
using scbot.services.zendesk;
using scbot.utils;

namespace scbot.processors.zendesk
{
    internal class ZendeskTicketCompareEngine
    {
        private readonly IListPersistenceApi<Tracked<ZendeskTicket>> m_Persistence;
        private const string c_PersistenceKey = "tracked-zd-tickets";

        public ZendeskTicketCompareEngine(IListPersistenceApi<Tracked<ZendeskTicket>> persistence)
        {
            m_Persistence = persistence;
        }

        public IEnumerable<Response> CompareTicketStates(IEnumerable<Update<ZendeskTicket>> comparison)
        {
            var differences = comparison.Select(x => new
            {
                differences = GetDifferenceString(x),
                update = x
            }).ToList();

            differences = differences.Where(x => x.differences != null).ToList();

            foreach (var diff in differences)
            {
                var id = diff.update.NewValue.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Value.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new Tracked<ZendeskTicket>(diff.update.NewValue, diff.update.Channel));
                Console.WriteLine("Diff: \nold: {0}\nnew:{1}", Json.Encode(diff.update.OldValue), Json.Encode(diff.update.NewValue));
            }
            return differences.Select(x => new Response(x.differences.Message, x.update.Channel, x.differences.Image));
        }

        private static Response GetDifferenceString(Update<ZendeskTicket> ttc)
        {
            var differences = GetDifferences(ttc).ToList();
            if (differences.Any())
            {
                var image = differences.Select(x => x.Image).FirstOrDefault(x => x.IsNotDefault());
                return new Response(string.Format("<https://redgatesupport.zendesk.com/agent/tickets/{0}|ZD#{0}> ({1}) updated: {2}",
                    ttc.NewValue.Id, ttc.NewValue.Description, string.Join(", ", differences.Select(x => x.Message))), null, image);
            }
            return null;
        }

        private static IEnumerable<Response> GetDifferences(Update<ZendeskTicket> x)
        {
            if (x.OldValue.Comments.Count < x.NewValue.Comments.Count)
            {
                yield return FormatCommentsAdded(x);
            }

            if (x.OldValue.Status != x.NewValue.Status)
            {
                yield return FormatStatusChanged(x);
            }

            if (x.OldValue.Description != x.NewValue.Description)
            {
                yield return FormatDescriptionChanged(x);
            }
        }

        private static Response FormatDescriptionChanged(Update<ZendeskTicket> x)
        {
            return new Response("description updated", null);
        }

        private static Response FormatStatusChanged(Update<ZendeskTicket> x)
        {
            return new Response(string.Format("`{0}` \u2192 `{1}`", x.OldValue.Status, x.NewValue.Status), null);
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
    }
}