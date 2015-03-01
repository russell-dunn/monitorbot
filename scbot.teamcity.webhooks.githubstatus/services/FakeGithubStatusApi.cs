using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace scbot.teamcity.webhooks.githubstatus.services
{
    class FakeGithubStatusApi : IGithubStatusApi
    {
        public Task SetStatus(string user, string repo, string commit, string status, string description, string context, string link)
        {
            var statusJson = Json.Encode(new
            {
                state = status,
                target_url = link,
                description = description,
                context = context
            });
            // want to make sure we get this right first
            Console.WriteLine("POST https://api.github.com/repos/{0}/{1}/statuses/{2} {3}", user, repo, commit, statusJson);
            return CompletedTask();
        }

        private static Task CompletedTask()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
