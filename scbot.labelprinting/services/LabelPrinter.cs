using System.Collections.Generic;
using System.Web.Helpers;
using scbot.core.utils;

namespace scbot.labelprinting
{
    public class LabelPrinter : ILabelPrinter
    {
        private readonly IWebClient m_WebClient;
        private readonly string m_PrintingApiUrl;

        public LabelPrinter(string printingApiUrl, IWebClient labelPrinting)
        {
            this.m_PrintingApiUrl = printingApiUrl;
            m_WebClient = labelPrinting;
        }

        public string PrintLabel(string title, List<string> images)
        {
            return PrintLabel(new { title = title, images = images });
        }

        public string PrintLabel(string title, string text, List<string> images)
        {
            return PrintLabel(new { title = title, text = text, images = images });
        }

        private string PrintLabel(object printRequest)
        {
            var response = m_WebClient.PostString(
                m_PrintingApiUrl, 
                Json.Encode(printRequest), 
                new[] { "content-type:application/json" }).Result;
            return response;
        }
    }
}