using Moq;
using NUnit.Framework;
using scbot;
using scbot.services;
using scbot.slack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fasttests.teamcity
{
    class TeamcityBuildTrackingTests
    {
        [Test]
        public void PostsUpdateWhenTrackedTeamcityBuildChangesState()
        {
            var teamcityBuildApi = new Mock<ITeamcityBuildApi>();
            var teamcityBuild1 = new TeamcityBuildStatus("12345", "build name", BuildState.Running);
            var teamcityBuild2 = new TeamcityBuildStatus("12345", "build name", BuildState.Finished);
            var persistence = new InMemoryKeyValueStore();
            var commandParser = new SlackCommandParser("scbot", "U123");

            teamcityBuildApi.Setup(x => x.GetBuild("12345")).ReturnsAsync(teamcityBuild1);

            var teamcityTracker = new TeamcityBuildTracker(commandParser, persistence, teamcityBuildApi.Object);

            teamcityTracker.ProcessMessage(new Message("a-channel", "a-user", "scbot track build#12345"));

            CollectionAssert.IsEmpty(teamcityTracker.ProcessTimerTick().Responses);

            teamcityBuildApi.Setup(x => x.GetBuild("12345")).ReturnsAsync(teamcityBuild2);

            var ping = teamcityTracker.ProcessTimerTick().Responses.Single();
            Assert.AreEqual("a-channel", ping.Channel);
            Assert.AreEqual("<http://teamcity/viewLog.html?buildId=12345|Build 12345> (build name) updated: build finished", ping.Message);

            // subsequent ticks should use updated values
            CollectionAssert.IsEmpty(teamcityTracker.ProcessTimerTick().Responses);
        }
    }
}
