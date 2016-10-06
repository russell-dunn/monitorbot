using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using monitorbot.core.utils;

namespace monitorbot.slack
{
    public class SlackRealTimeMessaging : ISlackRealTimeMessaging
    {
        private readonly StringClientWebSocket m_StringClientWebSocket;
        private readonly SlackInstanceInfo m_InstanceInfo;

        public SlackInstanceInfo InstanceInfo
        {
            get { return m_InstanceInfo; }
        }

        private SlackRealTimeMessaging(StringClientWebSocket webSocket, SlackInstanceInfo instanceInfo)
        {
            m_StringClientWebSocket = webSocket;
            m_InstanceInfo = instanceInfo;
        }

        public static async Task<SlackRealTimeMessaging> Connect(Uri wsUrl,
            SlackInstanceInfo instanceInfo, CancellationToken cancellationToken)
        {
            var ws = await StringClientWebSocket.Connect(wsUrl, cancellationToken);
            return new SlackRealTimeMessaging(ws, instanceInfo);
        }

        public async Task<string> Receive(CancellationToken cancellationToken)
        {
            var result = await m_StringClientWebSocket.ReceiveString(cancellationToken);
            //Trace.WriteLine("Got: "+result);
            return result;
        }

        public void Dispose()
        {
            m_StringClientWebSocket.Dispose();
        }
    }
}