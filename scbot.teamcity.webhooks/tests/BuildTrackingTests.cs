using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using scbot.core.bot;
using scbot.core.compareengine;
using scbot.core.persistence;
using scbot.core.tests;

namespace scbot.teamcity.webhooks.tests
{
    class BuildTrackingTests
    {
        private Mock<IHashPersistenceApi<Tracked<Build>>> m_TrackedBuilds;
        private Mock<IHashPersistenceApi<Tracked<Branch>>> m_TrackedBranches;

        [SetUp]
        public void SetUp()
        {
            m_TrackedBuilds = new Mock<IHashPersistenceApi<Tracked<Build>>>();
            m_TrackedBranches = new Mock<IHashPersistenceApi<Tracked<Branch>>>();
        }
        [Test]
        public void StartsTrackingBuild()
        {
            var commandParser = CommandlineParser.For("track build 12345");
            var processor = new TeamcityWebhooksMessageProcessor(m_TrackedBuilds.Object, m_TrackedBranches.Object, commandParser);
            var response = processor.ProcessMessage(new Message("a-channel", "a-user", "scbot track build 12345"));
            Assert.AreEqual("Now tracking build#12345", response.Responses.Single().Message);
            m_TrackedBuilds.Verify(x => x.Set("a-channel:::12345", new Tracked<Build>(new Build("12345"), "a-channel")));
        }

        [Test]
        public void StartsTrackingBreakagesOnBranch()
        {
            var commandParser = CommandlineParser.For("track breakages for branch master");
            var processor = new TeamcityWebhooksMessageProcessor(m_TrackedBuilds.Object, m_TrackedBranches.Object, commandParser);
            var response = processor.ProcessMessage(new Message("a-channel", "a-user", "a-message"));
            Assert.AreEqual("Now tracking breakages for branch master", response.Responses.Single().Message);
            m_TrackedBranches.Verify(x => x.Set("a-channel:::master", new Tracked<Branch>(new Branch(TeamcityEventTypes.BreakingBuilds, "master", null), "a-channel")));
        }

        [Test]
        public void AcceptsEventsToHandle()
        {
            var processor = new TeamcityWebhooksMessageProcessor(m_TrackedBuilds.Object, m_TrackedBranches.Object, new Mock<ICommandParser>().Object);
            processor.Accept(new TeamcityEvent("eventType", "buildId", "buildTypeId", "buildName", "buildResultDelta", "branchName"));
        }

        [Test]
        public void PostsResponseIfTrackingBuildId()
        {
            var trackedBuilds = new List<Tracked<Build>> {new Tracked<Build>(new Build("12345"), "a-channel")};
            var teamcityEvent = new TeamcityEvent("buildFinished", "12345", "bt1234", "the build", "unknown", "master");
            CollectionAssert.IsNotEmpty(new TeamcityEventHandler().GetResponseTo(teamcityEvent, trackedBuilds, new List<Tracked<Branch>>()));
        }

        [Test]
        public void PostsResponseIfTrackingBranchBreakage()
        {
            var trackedBranches = new List<Tracked<Branch>> { new Tracked<Branch>(new Branch(TeamcityEventTypes.BreakingBuilds, "master"), "a-channel") };
            var teamcityEvent = new TeamcityEvent("buildFinished", "12345", "bt1234", "the build", "broken", "master");
            CollectionAssert.IsNotEmpty(new TeamcityEventHandler().GetResponseTo(teamcityEvent, new List<Tracked<Build>>(), trackedBranches));
        }

        [Test]
        public void PostsResponseIfTrackingBuildFinish()
        {
            var trackedBranches = new List<Tracked<Branch>> { new Tracked<Branch>(new Branch(TeamcityEventTypes.FinishedBuilds, "asdf"), "a-channel") };
            var teamcityEvent = new TeamcityEvent("buildFinished", "12345", "bt1234", "the build", "unknown", "asdf");
            CollectionAssert.IsNotEmpty(new TeamcityEventHandler().GetResponseTo(teamcityEvent, new List<Tracked<Build>>(), trackedBranches));
        }

        [Test]
        public void DoesNotPostTwiceInTheSameChannel()
        {
            var commandParser = new Mock<ICommandParser>();
            var processor = new TeamcityWebhooksMessageProcessor(new InMemoryKeyValueStore(), commandParser.Object);
            processor.ProcessMessage(new Message("a-channel", "a-user", "m#1"), "branch", "master", "breakages");
            processor.ProcessMessage(new Message("a-channel", "a-user", "m#1"), "branch", "master", "breakages");
            processor.Accept(new TeamcityEvent("", "", "", "", "broken", "master"));

            var response = processor.ProcessTimerTick();
            Assert.AreEqual(1, response.Responses.Count());
        }

        [Test]
        public void WillPostTwiceInDifferentChannels()
        {
            var commandParser = new Mock<ICommandParser>();
            var processor = new TeamcityWebhooksMessageProcessor(new InMemoryKeyValueStore(), commandParser.Object);
            processor.ProcessMessage(new Message("a-channel",  "a-user", "m#1"), "branch", "master", "breakages");
            processor.ProcessMessage(new Message("a-channel2", "a-user", "m#1"), "branch", "master", "breakages");
            processor.Accept(new TeamcityEvent("", "", "", "", "broken", "master"));

            var response = processor.ProcessTimerTick();
            Assert.AreEqual(2, response.Responses.Count());
        }

        // TODO: general syntax is `track everything for build 12345` - "everything" should default to "broken" events only
    }

    [Flags]
    internal enum TeamcityEventTypes
    {
        None = 0,
        BreakingBuilds = 1 << 0,
        FinishedBuilds = 1 << 1,
        StartedBuilds = 1 << 2,
        CancelledBuilds = 1 << 3,
        All = BreakingBuilds | FinishedBuilds | StartedBuilds | CancelledBuilds
    }
}
