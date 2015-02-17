using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;
using scbot.services;

namespace scbot.processors
{
    internal class ZendeskTicketCompareEngine
    {
        private readonly IListPersistenceApi<TrackedTicket> m_Persistence;
        private const string c_PersistenceKey = "tracked-zd-tickets";

        public ZendeskTicketCompareEngine(IListPersistenceApi<TrackedTicket> persistence)
        {
            m_Persistence = persistence;
        }

        public IEnumerable<Response> CompareTicketStates(IEnumerable<TrackedTicketComparison> comparison)
        {
            var differences = comparison.Select(x => new
            {
                differences = GetDifferences(x),
                comparison = x
            }).ToList();

            foreach (var diff in differences.Where(x => x.differences.Any()))
            {
                var id = diff.comparison.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Ticket.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new TrackedTicket(diff.comparison.NewValue, diff.comparison.Channel));
                Console.WriteLine("Diff: \nold: {0}\nnew:{1}", Json.Encode(diff.comparison.OldValue), Json.Encode(diff.comparison.NewValue));
            }
            return differences.SelectMany(x => x.differences);
        }

        private static IEnumerable<Response> GetDifferences(TrackedTicketComparison x)
        {
            if (x.OldValue.Status != x.NewValue.Status ||
                x.OldValue.CommentCount != x.NewValue.CommentCount ||
                x.OldValue.Description != x.NewValue.Description)
            {
                yield return
                    new Response(
                        string.Format(
                            "Ticket <https://redgatesupport.zendesk.com/agent/tickets/{0}|ZD#{0}> was updated", x.Id),
                        x.Channel);

            }
        }
    }
}