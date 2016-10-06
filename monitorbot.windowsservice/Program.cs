using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace monitorbot.windowsservice
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            HostFactory.Run(x =>
            {
                x.Service<ScbotService>(sc =>
                {
                    sc.ConstructUsing(() => new ScbotService());

                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());
                });

                x.RunAsLocalService();

                x.SetDescription("https://github.com/red-gate/scbot");
                x.SetDisplayName("scbot");
                x.SetServiceName("ScbotService");
            });
        }
    }
}
