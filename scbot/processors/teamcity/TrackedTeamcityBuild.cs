using fasttests.teamcity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace scbot.processors.teamcity
{
    struct TrackedTeamcityBuild
    {
        public readonly TeamcityBuildStatus Build;
        public readonly string Channel;

        public TrackedTeamcityBuild(TeamcityBuildStatus build, string channel)
        {
            Build = build;
            Channel = channel;
        }
    }
}
