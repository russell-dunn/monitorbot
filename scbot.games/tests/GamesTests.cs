using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using scbot.core.persistence;
using scbot.core.tests;
using scbot.core.utils;

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
                "1: *Dave* (new rating - *1032* (*+32*), ladder position still - *1*) :star: #professional",
                "2: *Pete* (new rating - *1000* (*0*), new ladder position - *2* ⇩1)",
                "3: *Paul* (new rating - *968* (*-32*), new ladder position - *3* ⇩2)",

            };
            var responses = result.Responses.Select(x => x.Message).ToList();
            CollectionAssert.AreEqual(expected, responses);

            result = games.ProcessCommand("record worms game 1st Dave 2nd Pete 3rd Paul");
            expected = new[]
            {
                "1: *Dave* (new rating - *1060* (*+28*), ladder position still - *1*) :star: #professional",
                "2: *Pete* (new rating - *1000* (*0*), ladder position still - *2*)",
                "3: *Paul* (new rating - *940* (*-28*), ladder position still - *3*)",
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
                "1: *Dave* (rating 1062)",
                "2: *Pete* (rating 1001)",
                "3: *Larry* (rating 969)",
                "4: *Paul* (rating 968)",
            };
            CollectionAssert.AreEqual(expected, responses);
        }

        [Test]
        public void PlayersAreCaseInsensitive()
        {
            var games = MakeGames();
            games.ProcessCommand("record worms game 1st Dave 2nd Pete 3rd Paul");
            games.ProcessCommand("record worms game 1st DAVE 2nd PETE 3rd PAUL");
            var result = games.ProcessCommand("get worms leaderboard");
            var responses = result.Responses.Select(x => x.Message).ToList();
            var expected = new[]
            {
                "1: *Dave* (rating 1060)",
                "2: *Pete* (rating 1000)",
                "3: *Paul* (rating 940)",
            };
            CollectionAssert.AreEqual(expected, responses);
        }

        [Test]
        public void CanUseColonsInsteadOfOrdinals()
        {
            var games = MakeGames();
            games.ProcessCommand("record worms game 1:  Dave 2: Pete 3: Paul");
            var result = games.ProcessCommand("get worms leaderboard");
            var responses = result.Responses.Select(x => x.Message).ToList();
            var expected = new[]
            {
                "1: *Dave* (rating 1032)",
                "2: *Pete* (rating 1000)",
                "3: *Paul* (rating 968)",
            };
            CollectionAssert.AreEqual(expected, responses);
        }

        [Test]
        public void NoSpaceNeededAfterColons()
        {
            var games = MakeGames();
            games.ProcessCommand("record worms game 1:Dave 2:Pete 3:Paul");
            var result = games.ProcessCommand("get worms leaderboard");
            var responses = result.Responses.Select(x => x.Message).ToList();
            var expected = new[]
            {
                "1: *Dave* (rating 1032)",
                "2: *Pete* (rating 1000)",
                "3: *Paul* (rating 968)",
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

        [Test]
        public void UsesAliasesForDisplayNamesInResults()
        {
            var games = MakeGames();
            var result = games.ProcessCommand("record racing game 1st GreatestSwordsman 2nd Y.T. ");
            var expected = new[]
            {
                "1: *Hiro Protagonist* (new rating - *1032* (*+32*), ladder position still - *1*) :star: #professional",
                "2: *Yours Truly* (new rating - *968* (*-32*), new ladder position - *2* ⇩1)",
            };
            var responses = result.Responses.Select(x => x.Message).ToList();
            CollectionAssert.IsSubsetOf(expected, responses);
        }

        private GamesProcessor MakeGames()
        {
            var aliasList = new Mock<IAliasList>();
            aliasList.Setup(x => x.GetCanonicalNameFor(It.IsAny<string>()))
                .Returns<string>(name =>
                {
                    switch (name)
                    {
                        case "GreatestSwordsman": return "hiro.protagonist";
                        case "TheDeliverator": return "hiro.protagonist";
                        case "Y.T.": return "kourier.1992";
                        default: return name;
                    }
                });
            aliasList.Setup(x => x.GetDisplayNameFor(It.IsAny<string>()))
                .Returns<string>(name =>
                {
                    switch (name)
                    {
                        case "GreatestSwordsman": return "Hiro Protagonist";
                        case "TheDeliverator": return "Hiro Protagonist";
                        case "Y.T.": return "Yours Truly";
                        default: return name;
                    }
                });
            return new GamesProcessor(new InMemoryKeyValueStore(), aliasList.Object);
        }
    }
}
