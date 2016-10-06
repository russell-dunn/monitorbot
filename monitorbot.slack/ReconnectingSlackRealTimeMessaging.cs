using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace monitorbot.slack
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
                Trace.TraceError(wse.ToString());
            }

            for (int i = 0; i < 5; i++)
            {
                TryDisposeOld();
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
                Trace.WriteLine("Caught websocket exception .. reconnecting");
                try
                {
                    m_Underlying = await m_Factory();
                }
                catch (Exception e)
                {
                    Trace.TraceError("Failed to reconnect .. " + e);
                }
            }

            if (m_Underlying == null)
            {
                throw new Exception("Failed to reconnect to Slack after several tries");
            }

            return await m_Underlying.Receive(cancellationToken);
        }

        private void TryDisposeOld()
        {
            try
            {
                m_Underlying.Dispose();
                m_Underlying = null;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        public SlackInstanceInfo InstanceInfo { get { return m_Underlying.InstanceInfo; } }
    }
}
