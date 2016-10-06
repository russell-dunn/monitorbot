using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.core.utils
{
    public class WebClient : IWebClient
    {
        public async Task<string> DownloadString(string url, IEnumerable<string> headers = null)
        {
            using (var client = new System.Net.WebClient())
            {
                var utf8WithoutBom = new UTF8Encoding(false);
                client.Encoding = utf8WithoutBom;
                client.Headers.Add("user-agent: scbot/" + Version());
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.Headers.Add(header);
                    }
                }
                return await client.DownloadStringTaskAsync(url);
            }
        }

        public async Task<string> PostString(string url, string text, string[] headers = null)
        {
            using (var client = new System.Net.WebClient())
            {
                client.Headers.Add("user-agent: scbot/" + Version());
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        client.Headers.Add(header);
                    }
                }
                return await client.UploadStringTaskAsync(url, text);
            }
        }

        private static Version Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}
