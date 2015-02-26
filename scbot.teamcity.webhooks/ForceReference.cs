using Microsoft.Owin.Host.HttpListener;

namespace scbot.teamcity.webhooks
{
    static class ForceReference
    {
        private static void ToMicrosoftOwinHostHttpListener()
        {
            var unused = typeof (OwinHttpListener);
        }
    }
}
