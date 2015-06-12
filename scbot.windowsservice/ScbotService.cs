using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using scbot.core.utils;
using scbot.logging;

namespace scbot.windowsservice
{
    public partial class ScbotService : ServiceBase
    {
        private LoggingDashboard m_Dash;
        private Task m_Bot;
        private CancellationTokenSource m_StopTheBot;

        public ScbotService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var configuration = Configuration.Load();
            m_Dash = new LoggingDashboard(configuration.Get("logging-dashboard-endpoint"));
            m_StopTheBot = new CancellationTokenSource();
            m_Bot = scbot.Program.MainAsync(configuration, m_StopTheBot.Token);
        }

        protected override void OnStop()
        {
            m_StopTheBot.Cancel();
            m_Dash.Dispose();
        }
    }
}
