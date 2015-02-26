using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

namespace scbot.zendesk.services
{
    [DesignerCategory("Code")] // force VS to open this file with the code editor even though WebClient inherits Component
    internal class CookieAwareWebClient : WebClient
    {
        // based on http://stackoverflow.com/questions/14551345/accept-cookies-in-webclient

        private readonly CookieContainer m_CookieContainer;

        public CookieAwareWebClient(CookieContainer cookies)
        {
            m_CookieContainer = cookies;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var httpRequest = (HttpWebRequest)base.GetWebRequest(address);
            Debug.Assert(httpRequest != null, "httpRequest != null");
            httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            httpRequest.CookieContainer = m_CookieContainer;
            return httpRequest;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            Debug.Assert(response != null, "response != null");
            var setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

            if (setCookieHeader != null)
            {
                m_CookieContainer.SetCookies(response.ResponseUri, setCookieHeader);
            }
            return response;
        }
    }
}