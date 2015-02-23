using fasttests.teamcity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scbot.processors.teamcity
{
    struct TrackedTeamcityBuild
    {
        public readonly TeamcityBuildStatus BuildStatus;
        public readonly string Channel;

        public TrackedTeamcityBuild(TeamcityBuildStatus build, string channel)
        {
            BuildStatus = build;
            Channel = channel;
        }
    }
}
