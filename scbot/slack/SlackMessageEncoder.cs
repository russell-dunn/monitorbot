using System.Globalization;
using System.Threading;
using System.Web.Helpers;

namespace scbot.slack
{
    public class SlackMessageEncoder
    {
        private int m_NextId = 0;

        public string ToJSON(Response response)
        {
            return FixAngleBrackets(Json.Encode(new
            {
                id = NextId(),
                type = "message",
                text = response.Message,
                channel = response.Channel,
            }));
        }

        private static string FixAngleBrackets(string json)
        {
            // it looks like Json.Encode tries to be helpful ..

            return json.Replace("\\u003c", "<").Replace("\\u003e", ">");
        }

        private string NextId()
        {
            return Interlocked.Increment(ref m_NextId).ToString(CultureInfo.InvariantCulture);
        }
    }
}