using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scbot.processors.teamcity
{
    class TrackedTeamcityBuildComparison
    {
        public readonly string Channel;
        public readonly string Id;
        public readonly fasttests.teamcity.TeamcityBuildStatus OldValue;
        public readonly fasttests.teamcity.TeamcityBuildStatus NewValue;

        public TrackedTeamcityBuildComparison(string channel, string id, fasttests.teamcity.TeamcityBuildStatus oldValue, fasttests.teamcity.TeamcityBuildStatus newValue)
        {
            Channel = channel;
            Id = id;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
