using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.core.utils
{
    public interface IWebClient
    {
        Task<string> DownloadString(string url, IEnumerable<string> headers = null);
        Task<string> PostString(string url, string text, string[] headers = null);
    }
}
