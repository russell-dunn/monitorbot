using System;
using System.Net;
using HtmlAgilityPack;
using scbot.core.utils;

namespace scbot.htmltitles.services
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
            catch (Exception)
            {
                // TODO: log
                return null;
            }
        }
    }
}