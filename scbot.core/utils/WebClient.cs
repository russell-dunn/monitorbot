using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.utils
{
	public class WebClient : IWebClient
	{
		public async Task<string> DownloadString(string url)
		{
			using (var client = new System.Net.WebClient())
			{
				var utf8WithoutBom = new UTF8Encoding(false);
				client.Encoding = utf8WithoutBom;
				return await client.DownloadStringTaskAsync(url);
			}
		}
	}
}
