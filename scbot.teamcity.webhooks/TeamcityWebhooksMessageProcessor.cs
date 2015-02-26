using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.Owin.Hosting;
using Owin;
using scbot.bot;

namespace scbot.services.teamcity
{
    public class TeamcityWebhooksMessageProcessor : IMessageProcessor, IDisposable
    {
        private readonly IDisposable m_WebApp;
        // hack communication between OWIN instance and bot-created instance
        private static readonly ConcurrentQueue<string> s_Queue = new ConcurrentQueue<string>();

        private TeamcityWebhooksMessageProcessor(IDisposable webApp)
        {
            m_WebApp = webApp;
        }

        [Obsolete("Required by OWIN to call Configuration", true)]
        public TeamcityWebhooksMessageProcessor() { }

        public static TeamcityWebhooksMessageProcessor Start(string binding)
        {
            var webApp = WebApp.Start<TeamcityWebhooksMessageProcessor>(binding);
            return new TeamcityWebhooksMessageProcessor(webApp);
        }

        public void Configuration(IAppBuilder app)
        {
            app.Run(owinContext =>
            {
                var body = new StreamReader(owinContext.Request.Body).ReadToEnd();
                s_Queue.Enqueue(body);

                owinContext.Response.ContentType = "text/plain";
                return owinContext.Response.WriteAsync("thanks");
            });
        }

        public MessageResult ProcessTimerTick()
        {
            var result = new List<Response>();
            string nextJson;
            while (s_Queue.TryDequeue(out nextJson))
            {
                TeamcityEvent teamcityEvent = ParseTeamcityEvent(nextJson);
                if (teamcityEvent.BuildResultDelta == "broken" && teamcityEvent.BranchName == "master")
                {
                    result.Add(new Response(string.Format("{0}: Build {1} broke on master!", teamcityEvent.EventType, teamcityEvent.BuildName), "D03JWF44C"));
                }

                if (teamcityEvent.EventType == "buildFinished" && teamcityEvent.BranchName == "spike/guitests")
                {
                    result.Add(new Response(string.Format("{0} build finished", teamcityEvent.BuildName), "D03JWF44C"));
                }
            }
            return new MessageResult(result);
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

        public MessageResult ProcessMessage(Message message)
        {
            return MessageResult.Empty;
        }

        public void Dispose()
        {
            m_WebApp.Dispose();
        }
    }
}
