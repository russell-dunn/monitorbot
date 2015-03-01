using scbot.review.reviewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.services
{
    interface IReviewApi
    {
        Task<IEnumerable<DiffComment>> ReviewForCommit(string user, string repo, string hash);
        Task<IEnumerable<DiffComment>> ReviewForComparison(string user, string repo, string comparison);
        Task<IEnumerable<DiffComment>> ReviewForPullRequest(string user, string repo, int pr);
    }
}
