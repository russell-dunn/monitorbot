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
