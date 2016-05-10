using scbot.core.bot;
using scbot.core.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scbot.rg
{
    public class Installers : ICommandProcessor
    {
        public static IFeature Create(ICommandParser commandParser, IWebClient webClient)
        {
            return new BasicFeature("installers",
                "get a compare/data compare installer",
                "use `installer for <compare|data compare> <version>` to get a link to download the teamcity artifact for that build",
                new HandlesCommands(commandParser, new Installers(webClient)));
        }
        private readonly RegexCommandMessageProcessor m_Underlying;
        private readonly IWebClient m_WebClient;

        public Installers(IWebClient webClient)
        {
            m_WebClient = webClient;
            m_Underlying = new RegexCommandMessageProcessor(@"installer (for )?(sql )?(?<product>compare|data compare)(?<version> [0-9\.]+)?", InstallerFor);
        }

        public MessageResult InstallerFor(Command message, Match args)
        {
            var product = args.Group("product");
            var buildType = GetBuildType(product);
            var version = args.Group("version").Trim();
            var url = string.Format("http://teamcity.red-gate.com/guestAuth/app/rest/builds/buildType:{0}{1}/artifacts", buildType, String.IsNullOrEmpty(version) ? "" : ",number:"+version);
            dynamic json;
            try
            {
                json = m_WebClient.DownloadJson(url, new[] { "Accept: application/json" }).Result;
            }
            catch (Exception)
            {
                return Response.ToMessage(message, $"Could not find installer for {product} {version}");
            }
            var installer = new List<dynamic>(json.file).FirstOrDefault(x => x.name.EndsWith(".exe"));
            if (installer == null)
            {
                return Response.ToMessage(message, $"Could not find .exe artifact for {product} {version}");
            }
            var installerUrl = installer.content.href;
            var installerName = installer.name;
            return Response.ToMessage(message, string.Format("<http://teamcity.red-gate.com{0}|{1}>", installerUrl, installerName));
        }

        private static string GetBuildType(string product)
        {
            switch (product.ToLower())
            {
                case "compare":
                default:
                    return "bt2529";
                case "data compare":
                    return "bt2543";
            }
        }

        public MessageResult ProcessCommand(Command message)
        {
            return m_Underlying.ProcessCommand(message);
        }

        public MessageResult ProcessTimerTick()
        {
            return m_Underlying.ProcessTimerTick();
        }
    }
}
