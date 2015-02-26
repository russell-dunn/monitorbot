using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Host.HttpListener;

namespace scbot.teamcity
{
    static class ForceReference
    {
        private static void ToMicrosoftOwinHostHttpListener()
        {
            var unused = typeof (OwinHttpListener);
        }
    }
}
