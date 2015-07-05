using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using scbot.core.persistence;
using scbot.core.tests;

namespace scbot.games.tests
{
    class GamesTests
    {
        [Test]
        public void CanRecordArbitraryGame()
        {
            var games = MakeGames();
            var result = games.ProcessCommand("record worms game 1st Dave 2nd Pete 3rd Paul");
            var expected = new[]
            {
                "Creating new league `worms`",
                "Adding new player `Dave`",
                "Adding new player `Pete`",
                "Adding new player `Paul`",
                "1: *Dave* (new rating - 1022)",
                "2: *Pete* (new rating - 1000)",
                "3: *Paul* (new rating - 978)",

            };
            var responses = result.Responses.Select(x => x.Message).ToList();
            CollectionAssert.AreEqual(expected, responses);

            result = games.ProcessCommand("record worms game 1st Dave 2nd Pete 3rd Paul");
            expected = new[]
            {
                "1: *Dave* (new rating - 1041)",
                "2: *Pete* (new rating - 1000)",
                "3: *Paul* (new rating - 959)",
            };
            responses = result.Responses.Select(x => x.Message).ToList();
            CollectionAssert.AreEqual(expected, responses);
        }

        [Test]
        public void CanSeeOverallLeaderboard()
        {
            var games = MakeGames();
            games.ProcessCommand("record worms game 1st Dave 2nd Pete 3rd Paul");
            games.ProcessCommand("record worms game 1st Dave 2nd Pete 3rd Larry");
            var result = games.ProcessCommand("get worms leaderboard");
            var responses = result.Responses.Select(x => x.Message).ToList();
            var expected = new[]
            {
                "1: *Dave* (rating 1042)",
                "2: *Pete* (rating 1001)",
                "3: *Larry* (rating 979)",
                "4: *Paul* (rating 978)",
            };
            CollectionAssert.AreEqual(expected, responses);
        }

        [Test]
        public void ComplainsAboutEmptyLeaderboard()
        {
            var games = MakeGames();
            var result = games.ProcessCommand("get asdf leaderboard");
            var responses = result.Responses.Select(x => x.Message).ToList();
            var expected = new[]
            {
                "No games found for league `asdf`"
            };
            CollectionAssert.AreEqual(expected, responses);
        }

        [Test]
        public void ComplainsAboutBadlyFormattedResults()
        {
            var games = MakeGames();
            var result = games.ProcessCommand("record worms game 1x foo");
            Assert.AreEqual("Could not parse results `1x foo`", result.Responses.Single().Message);
        }

        [Test]
        public void ComplainsAboutMissingResults()
        {
            var games = MakeGames();
            var result = games.ProcessCommand("record worms game");
            Assert.AreEqual("Please provide some game results", result.Responses.Single().Message);
        }

        private GamesProcessor MakeGames()
        {
            return new GamesProcessor(new InMemoryKeyValueStore());
        }
    }
}
