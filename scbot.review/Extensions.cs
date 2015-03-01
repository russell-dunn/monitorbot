using scbot.review.diffparser;
using scbot.review.reviewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review
{
    public static class Extensions
    {
        internal static IEnumerable<DiffComment> Review(this IDiffReviewer reviewer, List<DiffLine> diffLines)
        {
            foreach (var line in diffLines)
            {
                line.Accept(reviewer);
            }
            return reviewer.Comments;
        }
    }
}
