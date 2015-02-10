using System.Dynamic;
using System.Globalization;
using System.Threading;
using System.Web.Helpers;

namespace scbot
{
    public class SlackMessageEncoder
    {
        private int m_NextId = 0;

        public string ToJSON(Response response)
        {
            return Json.Encode(new
            {
                id = NextId(),
                type = "message",
                text = response.Message,
                channel = response.Channel,
            });
        }

        private string NextId()
        {
            return Interlocked.Increment(ref m_NextId).ToString(CultureInfo.InvariantCulture);
        }
    }
}