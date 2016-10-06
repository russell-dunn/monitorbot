using System.Threading.Tasks;

namespace monitorbot.github.services
{
    public interface IGithubPRApi
    {
        Task<dynamic> PullRequest(string user, string repo, int prNum);
    }
}