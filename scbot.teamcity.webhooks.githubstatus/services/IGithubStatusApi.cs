using System.Threading.Tasks;

namespace scbot.teamcity.webhooks.githubstatus.services
{
    interface IGithubStatusApi
    {
        Task SetStatus(string user, string repo, string commit, string status, string description, string context, string link);
    }
}