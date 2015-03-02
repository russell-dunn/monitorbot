using NUnit.Framework;
using scbot.core.utils;
using scbot.teamcity.webhooks.githubstatus.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.teamcity.webhooks.githubstatus
{
    class Tests
    {
        [Test, Explicit]
        public async void CanFetchChanges()
        {
            var api = new TeamcityChangesApi(new Time(), new TeamcityBuildJsonApi(new WebClient(), null));
            var result = await api.RevisionForBuild("6004527");
            Console.WriteLine(result.BuildId + " " + result.Repo + " " + result.Hash);
        }

    }
}
