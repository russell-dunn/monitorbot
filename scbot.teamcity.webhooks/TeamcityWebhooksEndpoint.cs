using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using Microsoft.Owin.Hosting;
using Owin;

namespace scbot.teamcity.webhooks
{
    public static class TeamcityWebhooksEndpoint
    {
        public static IDisposable Start(string binding, IEnumerable<IAcceptTeamcityEvents> handlers)
        {
            handlers = handlers.ToList();
            var webApp = WebApp.Start(binding, app => app.Run(owinContext =>
            {
                var body = new StreamReader(owinContext.Request.Body).ReadToEnd();
                var teamcityEvent = ParseTeamcityEvent(body);

                if (teamcityEvent != null)
                foreach (var handler in handlers)
                {
                    handler.Accept(teamcityEvent);
                }

                owinContext.Response.ContentType = "text/plain";
                return owinContext.Response.WriteAsync("thanks");
            }));
            return webApp;
        }

        private static TeamcityEvent ParseTeamcityEvent(string eventJson)
        {
            try
            {
                var build = Json.Decode(eventJson).build;
                return new TeamcityEvent(
                    build.notifyType,
                    build.buildId,
                    build.buildTypeId,
                    build.buildName,
                    build.buildResultDelta,
                    build.branchName
                    );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
