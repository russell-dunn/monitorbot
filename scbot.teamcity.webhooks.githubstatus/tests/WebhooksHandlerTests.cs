using Moq;
using NUnit.Framework;
using scbot.teamcity.webhooks.githubstatus.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.teamcity.webhooks.endpoint;

namespace scbot.teamcity.webhooks.githubstatus.tests
{
    class WebhooksHandlerTests
    {
        [Test]
        public void AsksTeamcityApiForRepoAndCommit()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.Unknown, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "foo", TeamcityBuildState.Unknown));

            teamcityApi.Verify(x => x.RevisionForBuild("build-id"));
        }

        [Test]
        public void IgnoresMasterBranch()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.Unknown, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "master", TeamcityBuildState.Unknown));

            teamcityApi.Verify(x => x.RevisionForBuild("build-id"), Times.Never);
        }

        [Test]
        public void PostsStatusWhenBuildStarts()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            teamcityApi.Setup(x => x.RevisionForBuild("build-id")).ReturnsAsync(new TeamcityRevisionForBuild("build-id", "a-user", "a-repo", "123hash"));
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.BuildStarted, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "foo", TeamcityBuildState.Unknown));

            var url = "http://buildserver/viewLog.html?buildId=build-id";
            githubApi.Verify(x => x.SetStatus("a-user", "a-repo", "123hash", "pending", "build started", "build name", url));
        }
    }
}
