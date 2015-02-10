using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace scbot.slack
{
    public class SlackRealTimeMessaging : IDisposable
    {
        private readonly StringClientWebSocket m_StringClientWebSocket;
        public readonly string BotId;

        private SlackRealTimeMessaging(StringClientWebSocket webSocket, string botId)
        {
            m_StringClientWebSocket = webSocket;
            BotId = botId;
        }

        public static async Task<SlackRealTimeMessaging> Connect(Uri wsUrl, string botId, CancellationToken cancellationToken)
        {
            var ws = await StringClientWebSocket.Connect(wsUrl, cancellationToken);
            return new SlackRealTimeMessaging(ws, botId);
        }

        public async Task Send(string str, CancellationToken cancellationToken)
        {
            Console.WriteLine("Sending: "+str);
            await m_StringClientWebSocket.SendString(str, cancellationToken);
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