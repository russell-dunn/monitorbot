using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace scbot.slack
{
    public class ReconnectingSlackRealTimeMessaging : ISlackRealTimeMessaging
    {
        private ISlackRealTimeMessaging m_Underlying;
        private readonly Func<Task<ISlackRealTimeMessaging>> m_Factory;

        private ReconnectingSlackRealTimeMessaging(ISlackRealTimeMessaging underlying, Func<Task<ISlackRealTimeMessaging>> factory)
        {
            m_Underlying = underlying;
            m_Factory = factory;
        }

        public static async Task<ISlackRealTimeMessaging> CreateAsync(Func<Task<ISlackRealTimeMessaging>> factory)
        {
            var underlying = await factory();
            return new ReconnectingSlackRealTimeMessaging(underlying, factory);
        }

        public void Dispose()
        {
            m_Underlying.Dispose();
        }

        public async Task<string> Receive(CancellationToken cancellationToken)
        {
            try
            {
                return await m_Underlying.Receive(cancellationToken);
            }
            catch (WebSocketException wse)
            {
                Trace.WriteLine(wse);
            }

            Trace.WriteLine("Caught websocket exception .. reconnecting");
            // reconnect and retry once
            TryDisposeOld();
            m_Underlying = await m_Factory();
            return await m_Underlying.Receive(cancellationToken);
        }

        private void TryDisposeOld()
        {
            try
            {
                m_Underlying.Dispose();
            }
            catch (Exception)
            {
                ;
            }
        }

        public string BotId { get { return m_Underlying.BotId; } }
    }
}
