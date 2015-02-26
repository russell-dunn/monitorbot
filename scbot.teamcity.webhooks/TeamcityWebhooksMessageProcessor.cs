using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.Owin.Hosting;
using Owin;
using scbot.bot;
using scbot.services.compareengine;
using scbot.services.persistence;
using scbot.teamcity.webhooks;
using scbot.teamcity.webhooks.tests;
using scbot.utils;

namespace scbot.services.teamcity
{
    public class TeamcityWebhooksMessageProcessor : IMessageProcessor, IDisposable
    {
        private const string c_TrackedBuilds = "tcwh-tracked-builds";
        private readonly IListPersistenceApi<Tracked<Build>> m_ListPersistenceApi;
        private readonly ICommandParser m_CommandParser;
        private readonly IDisposable m_WebApp;
        // hack communication between OWIN instance and bot-created instance
        private static readonly ConcurrentQueue<string> s_Queue = new ConcurrentQueue<string>();

        private static readonly Regex s_TrackRegex = new Regex(@"(?<trackType>(build))\s+(?<trackItem>([0-9]{5,10}))");
        private readonly TeamcityEventHandler m_TeamcityEventHandler;

        private TeamcityWebhooksMessageProcessor(IListPersistenceApi<Tracked<Build>> listPersistenceApi, ICommandParser commandParser, IDisposable webApp)
        {
            m_ListPersistenceApi = listPersistenceApi;
            m_CommandParser = commandParser;
            m_WebApp = webApp;
            m_TeamcityEventHandler = new TeamcityEventHandler();
        }

        [Obsolete("Required by OWIN to call Configuration", true)]
        public TeamcityWebhooksMessageProcessor() { }

        internal static TeamcityWebhooksMessageProcessor Start(IListPersistenceApi<Tracked<Build>> listPersistenceApi, ICommandParser commandParser, string binding)
        {
            var webApp = WebApp.Start<TeamcityWebhooksMessageProcessor>(binding);
            return new TeamcityWebhooksMessageProcessor(listPersistenceApi, commandParser, webApp);
        }

        public static IMessageProcessor Start(IKeyValueStore kvs, ICommandParser commandParser, string binding)
        {
            return Start(new ListPersistenceApi<Tracked<Build>>(kvs), commandParser, binding);
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
            var trackedBuilds = m_ListPersistenceApi.ReadList(c_TrackedBuilds);
            while (s_Queue.TryDequeue(out nextJson))
            {
                var teamcityEvent = ParseTeamcityEvent(nextJson);
                result.AddRange(m_TeamcityEventHandler.GetResponseTo(teamcityEvent, trackedBuilds));
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
            string toTrack;
            Match trackMatch;
            if (m_CommandParser.TryGetCommand(message, "track", out toTrack) && s_TrackRegex.TryMatch(toTrack, out trackMatch))
            {
                switch (trackMatch.Groups["trackType"].ToString())
                {
                    case "build": return TrackBuild(message, trackMatch.Groups["trackItem"].ToString());
                }
            }
            return MessageResult.Empty;
        }

        private MessageResult TrackBuild(Message message, string buildId)
        {
            m_ListPersistenceApi.AddToList(c_TrackedBuilds, new Tracked<Build>(new Build(buildId), message.Channel));
            return new MessageResult(new[]{Response.ToMessage(message, string.Format("Now tracking build#{0}", buildId))});
        }

        public void Dispose()
        {
            m_WebApp.Dispose();
        }
    }
}
