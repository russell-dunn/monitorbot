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
    public class Installers : IMessageProcessor
    {
        public static IFeature Create(ICommandParser commandParser, IWebClient webClient)
        {
            return new BasicFeature("installers",
                "get a compare/data compare installer",
                "use `installer for <compare|data compare> <version>` to get a link to download the teamcity artifact for that build",
                new Installers(commandParser, webClient));
        }
        private readonly RegexCommandMessageProcessor m_Underlying;
        private readonly IWebClient m_WebClient;

        public Installers(ICommandParser commandParser, IWebClient webClient)
        {
            m_WebClient = webClient;
            m_Underlying = new RegexCommandMessageProcessor(commandParser, @"installer (for )?(sql )?(?<product>compare|data compare)(?<version> [0-9\.]+)?", InstallerFor);
        }

        public MessageResult InstallerFor(Message message, Match args)
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
                return Response.ToMessage(message, string.Format("Could not find installer for {0} {1}", product, version));
            }
            var installerUrl = json.file[0].content.href;
            var installerName = json.file[0].name;
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

        public MessageResult ProcessMessage(Message message)
        {
            return ((IMessageProcessor)m_Underlying).ProcessMessage(message);
        }

        public MessageResult ProcessTimerTick()
        {
            return ((IMessageProcessor)m_Underlying).ProcessTimerTick();
        }
    }
}
