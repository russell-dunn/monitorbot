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
                differences = GetDifferenceString(x),
                comparison = x
            }).ToList();

            differences = differences.Where(x => x.differences != null).ToList();

            foreach (var diff in differences)
            {
                var id = diff.comparison.Id;
                m_Persistence.RemoveFromList(c_PersistenceKey, x => x.Ticket.Id == id);
                m_Persistence.AddToList(c_PersistenceKey, new TrackedTicket(diff.comparison.NewValue, diff.comparison.Channel));
                Console.WriteLine("Diff: \nold: {0}\nnew:{1}", Json.Encode(diff.comparison.OldValue), Json.Encode(diff.comparison.NewValue));
            }
            return differences.Select(x => new Response(x.differences, x.comparison.Channel));
        }

        private static string GetDifferenceString(TrackedTicketComparison x)
        {
            var differences = GetDifferences(x).ToList();
            if (differences.Any())
            {
                return string.Format("<https://redgatesupport.zendesk.com/agent/tickets/{0}|ZD#{0}> ({1}) updated: {2}",
                    x.Id, x.NewValue.Description, string.Join(", ", differences));
            }
            return null;
        }

        private static IEnumerable<string> GetDifferences(TrackedTicketComparison x)
        {
            if (x.OldValue.CommentCount < x.NewValue.CommentCount)
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

        private static string FormatDescriptionChanged(TrackedTicketComparison x)
        {
            return "description updated";
        }

        private static string FormatStatusChanged(TrackedTicketComparison x)
        {
            return string.Format("`{0}`\u2192`{1}`", x.OldValue.Status, x.NewValue.Status);
        }

        private static string FormatCommentsAdded(TrackedTicketComparison x)
        {
            var diff = (x.NewValue.CommentCount - x.OldValue.CommentCount);
            if (diff == 1) return "comment added";
            return string.Format("{0} comments added", diff);
        }
    }
}