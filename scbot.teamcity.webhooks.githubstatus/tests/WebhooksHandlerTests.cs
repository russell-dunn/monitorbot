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
            handler.Accept(new TeamcityEvent(TeamcityEventType.Unknown, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "foo", TeamcityBuildState.Unknown, "", "1.0"));

            teamcityApi.Verify(x => x.RevisionForBuild("build-id"));
        }

        [Test]
        public void IgnoresMasterBranch()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.Unknown, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "master", TeamcityBuildState.Unknown, "", "1.0"));

            teamcityApi.Verify(x => x.RevisionForBuild("build-id"), Times.Never);
        }

        [Test]
        public void PostsStatusWhenBuildStarts()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            teamcityApi.Setup(x => x.RevisionForBuild("build-id")).ReturnsAsync(new TeamcityRevisionForBuild("build-id", "a-user", "a-repo", "123hash"));
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.BuildStarted, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "foo", TeamcityBuildState.Unknown, "", "1.0"));

            var url = "http://buildserver/viewLog.html?buildId=build-id";
            githubApi.Verify(x => x.SetStatus("a-user", "a-repo", "123hash", "pending", "build started", "build name", url));
        }

        [Test]
        public void PostsSuccessStatusWhenBuildFinishes()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            teamcityApi.Setup(x => x.RevisionForBuild("build-id")).ReturnsAsync(new TeamcityRevisionForBuild("build-id", "a-user", "a-repo", "123hash"));
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.BuildFinished, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "foo", TeamcityBuildState.Success, "tests passed", "1.0"));

            var url = "http://buildserver/viewLog.html?buildId=build-id";
            githubApi.Verify(x => x.SetStatus("a-user", "a-repo", "123hash", "success", "tests passed", "build name", url)); 
        }

        [Test]
        public void PostsFailureStatusWhenBuildFails()
        {
            var teamcityApi = new Mock<ITeamcityChangesApi>();
            teamcityApi.Setup(x => x.RevisionForBuild("build-id")).ReturnsAsync(new TeamcityRevisionForBuild("build-id", "a-user", "a-repo", "123hash"));
            var githubApi = new Mock<IGithubStatusApi>();
            var handler = new StatusWebhooksHandler(githubApi.Object, teamcityApi.Object);
            handler.Accept(new TeamcityEvent(TeamcityEventType.BuildFinished, "build-id", "buildType", "build name", BuildResultDelta.Unknown, "foo", TeamcityBuildState.Failure, "Error message is logged", "1.0"));

            var url = "http://buildserver/viewLog.html?buildId=build-id";
            githubApi.Verify(x => x.SetStatus("a-user", "a-repo", "123hash", "failure", "Error message is logged", "build name", url));
        }
    }
}
