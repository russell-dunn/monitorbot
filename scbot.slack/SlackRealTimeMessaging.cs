using System;
using System.Threading;
using System.Threading.Tasks;
using scbot.utils;

namespace scbot.slack
{
    public class SlackRealTimeMessaging : ISlackRealTimeMessaging
    {
        private readonly StringClientWebSocket m_StringClientWebSocket;
        private readonly string m_BotId;

        public string BotId
        {
            get { return m_BotId; }
        }

        private SlackRealTimeMessaging(StringClientWebSocket webSocket, string botId)
        {
            m_StringClientWebSocket = webSocket;
            m_BotId = botId;
        }

        public static async Task<SlackRealTimeMessaging> Connect(Uri wsUrl, string botId, CancellationToken cancellationToken)
        {
            var ws = await StringClientWebSocket.Connect(wsUrl, cancellationToken);
            return new SlackRealTimeMessaging(ws, botId);
        }

        public async Task<string> Receive(CancellationToken cancellationToken)
        {
            var result = await m_StringClientWebSocket.ReceiveString(cancellationToken);
            Console.WriteLine("Got: "+result);
            return result;
        }

        public void Dispose()
        {
            m_StringClientWebSocket.Dispose();
        }
    }
}