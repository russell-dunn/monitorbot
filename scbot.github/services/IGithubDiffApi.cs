using System.Threading.Tasks;

namespace scbot.review.services
{
    public interface IGithubDiffApi
    {
        Task<dynamic> DiffForCommit(string user, string repo, string hash);
        Task<dynamic> DiffForComparison(string user, string repo, string comparison);
        Task<dynamic> DiffForPullRequest(string user, string repo, int pr);
    }
}
