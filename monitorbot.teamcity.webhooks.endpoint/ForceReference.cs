using Microsoft.Owin.Host.HttpListener;

namespace monitorbot.teamcity.webhooks.endpoint
{
    static class ForceReference
    {
        private static void ToMicrosoftOwinHostHttpListener()
        {
            var unused = typeof (OwinHttpListener);
        }
    }
}
