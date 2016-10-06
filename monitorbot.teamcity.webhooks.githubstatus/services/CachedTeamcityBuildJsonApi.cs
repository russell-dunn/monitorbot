using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.core.utils;

namespace monitorbot.teamcity.webhooks.githubstatus.services
{
    class CachedTeamcityBuildJsonApi : ITeamcityBuildJsonApi
    {
        private readonly Cache<string, dynamic> m_Cache;
        private readonly ITeamcityBuildJsonApi m_Underlying;

        public CachedTeamcityBuildJsonApi(ITime time, ITeamcityBuildJsonApi underlying)
        {
            m_Cache = new Cache<string, dynamic>(time, TimeSpan.FromDays(1));
            m_Underlying = underlying;
        }

        public Task<dynamic> Build(string buildId)
        {
            var cached = m_Cache.Get(buildId);
            if (cached == null)
            {
                m_Cache.Set(buildId, m_Underlying.Build(buildId));
            }
            return m_Cache.Get(buildId);
        }
    }
}
