using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace monitorbot.core.utils
{
    public class HasteServerPasteBin : IPasteBin
    {
        private readonly string m_BaseUrl;
        private readonly IWebClient m_WebClient;

        public HasteServerPasteBin(IWebClient webClient, string baseUrl)
        {
            m_WebClient = webClient;
            m_BaseUrl = baseUrl;
        }

        public string UploadPaste(string data)
        {
            var result = m_WebClient.PostString(m_BaseUrl + "/documents", data).Result;
            var json = Json.Decode(result);
            return m_BaseUrl + "/" + json.key + ".txt";
        }
    }
}
