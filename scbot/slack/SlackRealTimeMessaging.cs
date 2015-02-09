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
        private readonly ClientWebSocket m_WebSocket;

        private SlackRealTimeMessaging(ClientWebSocket webSocket)
        {
            m_WebSocket = webSocket;
        }

        public static async Task<SlackRealTimeMessaging> Connect(Uri wsUrl, CancellationToken cancellationToken)
        {
            var webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(wsUrl, cancellationToken);
            return new SlackRealTimeMessaging(webSocket);
        }

        public async Task<dynamic> Receive(CancellationToken cancellationToken)
        {
            var json = await ReceiveString(cancellationToken);
            return Json.Decode(json);
        }

        private async Task<string> ReceiveString(CancellationToken cancellationToken)
        {
            // code based on https://msdn.microsoft.com/en-us/magazine/jj863133.aspx
            var buffer = new byte[1024];
            while (true)
            {
                var segment = new ArraySegment<byte>(buffer);
                var result =
                    await m_WebSocket.ReceiveAsync(segment, cancellationToken);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await m_WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK",
                        cancellationToken);
                    return null;
                }
                if (result.MessageType == WebSocketMessageType.Binary)
                {
                    await m_WebSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType,
                        "I don't do binary", cancellationToken);
                    return null;
                }
                int count = result.Count;
                while (!result.EndOfMessage)
                {
                    if (count >= buffer.Length)
                    {
                        await m_WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                            "That's too long", cancellationToken);
                        return null;
                    }
                    segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                    result = await m_WebSocket.ReceiveAsync(segment, cancellationToken);
                    count += result.Count;
                }
                var message = new UTF8Encoding(false).GetString(buffer, 0, count);
                return message;
            }
        }

        public void Dispose()
        {
            if (m_WebSocket != null) m_WebSocket.Dispose();
        }
    }
}