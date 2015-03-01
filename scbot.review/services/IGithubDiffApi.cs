using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review
{
    interface IGithubDiffApi
    {
        Task<dynamic> DiffForCommit(string user, string repo, string hash);
        Task<dynamic> DiffForComparison(string user, string repo, string comparison);
        Task<dynamic> DiffForPullRequest(string user, string repo, int pr);
    }
}
