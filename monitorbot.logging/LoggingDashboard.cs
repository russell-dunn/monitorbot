using OwinLoggingDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monitorbot.logging
{
    public class LoggingDashboard : IDisposable
    {
        private Dashboard m_Dash;

        public LoggingDashboard(string endpoint)
        {
            m_Dash = Dashboard.Start(endpoint);
        }

        public void Dispose()
        {
            m_Dash.Dispose();
        }
    }
}
