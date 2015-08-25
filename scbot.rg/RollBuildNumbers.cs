using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot.core.bot;
using scbot.core.utils;
using WebClient = System.Net.WebClient;

namespace scbot.release
{
    public class RollBuildNumbers : ICommandProcessor
    {
        public static IFeature Create(ICommandParser commandParser, Configuration configuration)
        {
            return new BasicFeature("rollbuildnumbers",
                "increment the Compare teamcity build numbers after a release",
                "use `roll build numbers` to increment the current Compare minor version (eg `11.1.20` -> `11.2.1`)",
                new HandlesCommands(commandParser, new RollBuildNumbers(configuration.Get("teamcity-auth"))));
        }
        private readonly string m_TeamcityCredentials;
        private readonly RegexCommandMessageProcessor m_Underlying;

        public RollBuildNumbers(string teamcityCredentials)
        {
            m_TeamcityCredentials = teamcityCredentials;
            m_Underlying = new RegexCommandMessageProcessor("^roll build number(s)?$", RollBuildNumber);
        }

        public MessageResult RollBuildNumber(Command message, Match args)
        {
            using (var webClient = new WebClient())
            {
                var creds = m_TeamcityCredentials.Split(new[] { ':' }, 2);
                webClient.Credentials = new NetworkCredential(creds[0], creds[1]);
                webClient.Headers.Add("content-type", "text/plain");

                var baseUrl = "http://teamcity.red-gate.com/httpAuth/app/rest/9.0/{0}";

                string projectVersionGetUrl = string.Format(baseUrl,
                    "projects/id:SqlCompareDataCompareStaging/parameters/system.BranchVersion/value");
                string projectVersionSetUrl = string.Format(baseUrl,
                    "projects/id:SqlCompareDataCompareStaging/parameters/system.BranchVersion");
                var buildTypes = new[] { "bt2514", "SqlCompareDataCompareStaging_SQLCompareUIs" };
                try
                {
                    var branchVersion = webClient.DownloadString(projectVersionGetUrl);

                    var parsedVersion = new Version(branchVersion);

                    var newVersion = new Version(parsedVersion.Major, parsedVersion.Minor, parsedVersion.Build + 1);

                    webClient.UploadString(projectVersionSetUrl, "PUT", newVersion.ToString(3));

                    foreach (var bt in buildTypes)
                    {
                        var btCounterUrl = string.Format(baseUrl,
                            "buildTypes/id:" + bt + "/settings/buildNumberCounter");
                        webClient.UploadString(btCounterUrl, "PUT", "1");
                    }

                    return Response.ToMessage(message,
                        string.Format("Current compare version: {0} .. now set to {1}\nBuild counters reset to 1",
                            parsedVersion.ToString(3), newVersion.ToString(3)));
                }
                catch (WebException e)
                {
                    Trace.TraceError(e.ToString());
                    return Response.ToMessage(message,
                        "Error from teamcity:\n" + new StreamReader(e.Response.GetResponseStream()).ReadToEnd());
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    return Response.ToMessage(message, "Oops: " + e.Message);
                }
            }
        }

        public MessageResult ProcessCommand(Command message)
        {
            return m_Underlying.ProcessCommand(message);
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }
    }
}
