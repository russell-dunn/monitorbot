using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Helpers;
using Microsoft.Owin.Hosting;
using Owin;

namespace scbot.teamcity.webhooks.endpoint
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
                    GetEventType(build.notifyType),
                    build.buildId,
                    build.buildTypeId,
                    build.buildName,
                    GetBuildResultDelta(build.buildResultDelta),
                    build.branchName,
                    GetBuildState(build.buildResult),
                    build.buildStatus,
                    build.buildNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static TeamcityBuildState GetBuildState(string buildResult)
        {
            switch (buildResult)
            {
                case "running": return TeamcityBuildState.Running;
                case "success": return TeamcityBuildState.Success;
                case "failure": return TeamcityBuildState.Failure;
                default: return TeamcityBuildState.Unknown;
            }
        }

        private static TeamcityEventType GetEventType(string notifyType)
        {
            switch (notifyType)
            {
                case "buildStarted": return TeamcityEventType.BuildStarted;
                case "buildInterrupted": return TeamcityEventType.BuildInterrupted;
                case "beforeBuildFinish": return TeamcityEventType.BeforeBuildFinish;
                case "buildFinished": return TeamcityEventType.BuildFinished;
                default: return TeamcityEventType.Unknown;
            }
        }

        private static BuildResultDelta GetBuildResultDelta(string buildResultDelta)
        {
            switch (buildResultDelta)
            {
                case "unchanged": return BuildResultDelta.Unchanged;
                case "broken": return BuildResultDelta.Broken;
                case "fixed": return BuildResultDelta.Fixed;
                case "unknown": default: return BuildResultDelta.Unknown;
            }
        }
    }
}
