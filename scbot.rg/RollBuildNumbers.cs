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
    public class RollBuildNumbers : IMessageProcessor
    {
        private readonly string m_TeamcityCredentials;
        private readonly RegexCommandMessageProcessor m_Underlying;

        public RollBuildNumbers(ICommandParser commandParser, string teamcityCredentials)
        {
            m_TeamcityCredentials = teamcityCredentials;
            m_Underlying = new RegexCommandMessageProcessor(commandParser, "^roll build number(s)?$", RollBuildNumber);
        }

        public MessageResult RollBuildNumber(Message message, Match args)
        {
            using (var webClient = new WebClient())
            {
                var creds = m_TeamcityCredentials.Split(new[] { ':' }, 2);
                webClient.Credentials = new NetworkCredential(creds[0], creds[1]);

                var baseUrl = "http://teamcity.red-gate.com/httpAuth/app/rest/9.0/{0}";

                string projectVersionGetUrl = string.Format(baseUrl,
                    "projects/id:SqlCompareDataCompareStaging/parameters/system.BranchVersion/value");
                string projectVersionSetUrl = string.Format(baseUrl,
                    "projects/id:SqlCompareDataCompareStaging/parameters/system.BranchVersion");
                var buildTypes = new[] { "bt2530", "bt2535", "bt2514", "SqlCompareDataCompareStaging_SQLCompareUIs" };
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

        public MessageResult ProcessMessage(Message message)
        {
            return m_Underlying.ProcessMessage(message);
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }
    }
}
