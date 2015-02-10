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

        private SlackRealTimeMessaging(StringClientWebSocket webSocket)
        {
            m_StringClientWebSocket = webSocket;
        }

        public static async Task<SlackRealTimeMessaging> Connect(Uri wsUrl, CancellationToken cancellationToken)
        {
            var ws = await StringClientWebSocket.Connect(wsUrl, cancellationToken);
            return new SlackRealTimeMessaging(ws);
        }

        public async Task Send(string str, CancellationToken cancellationToken)
        {
            await m_StringClientWebSocket.SendString(str, cancellationToken);
        }

        public async Task<string> Receive(CancellationToken cancellationToken)
        {
            return await m_StringClientWebSocket.ReceiveString(cancellationToken);
        }

        public void Dispose()
        {
            m_StringClientWebSocket.Dispose();
        }
    }
}