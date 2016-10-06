using System;
using NUnit.Framework;
using System.Diagnostics;
using monitorbot.core.utils;
using monitorbot.teamcity.webhooks.githubstatus.services;

namespace monitorbot.teamcity.webhooks.githubstatus.tests
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
