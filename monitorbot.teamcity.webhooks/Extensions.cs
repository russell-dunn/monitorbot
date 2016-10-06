using System.Text.RegularExpressions;
using monitorbot.core.compareengine;
using monitorbot.teamcity.webhooks.tests;

namespace monitorbot.teamcity.webhooks
{
    public static class Extensions
    {
        internal static bool IsTracking(this Tracked<Branch> trackedBranch, TeamcityEventTypes types)
        {
            return trackedBranch.Value.TrackedEventTypes.HasFlag(types);
        }
    }
}
