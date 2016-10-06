using System;
using System.Net;
using HtmlAgilityPack;
using System.Diagnostics;
using monitorbot.core.utils;

namespace monitorbot.htmltitles.services
{
    public class HtmlTitleParser : IHtmlTitleParser
    {
        private readonly IWebClient m_WebClient;

        public HtmlTitleParser(IWebClient webClient)
        {
            m_WebClient = webClient;
        }

        public string GetHtmlTitle(string url)
        {
            try
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(m_WebClient.DownloadString(url).Result);
                var title = doc.DocumentNode.SelectSingleNode("//title");
                return WebUtility.HtmlDecode(title.InnerText);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                return null;
            }
        }
    }
}