using System.Threading.Tasks;

namespace scbot.github.services
{
    public interface IGithubPRApi
    {
        Task<dynamic> PullRequest(string user, string repo, int prNum);
    }
}