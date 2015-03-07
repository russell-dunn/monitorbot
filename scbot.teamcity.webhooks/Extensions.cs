using System.Text.RegularExpressions;
using scbot.core.compareengine;
using scbot.teamcity.webhooks.tests;

namespace scbot.teamcity.webhooks
{
    public static class Extensions
    {
        internal static bool IsTracking(this Tracked<Branch> trackedBranch, TeamcityEventTypes types)
        {
            return trackedBranch.Value.TrackedEventTypes.HasFlag(types);
        }
    }
}
