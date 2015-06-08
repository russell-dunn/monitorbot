using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MoreLinq;
using scbot.core.bot;
using scbot.core.utils;

namespace scbot.polls
{
    public class Polls : ICommandProcessor
    {
        public static IFeature Create(ICommandParser parser)
        {
            return new BasicFeature("polls",
                "run a poll to enact the tyranny of the majority",
                "use `start poll` to start a poll",
                new HandlesCommands(parser, new Polls()));
        }

        private readonly RegexCommandMessageProcessor m_Underlying;

        private bool m_PollRunning;
        private string m_CurrentPollChannel;
        private List<string> m_CurrentPollChoices;
        private Dictionary<string, int> m_CurrentPollVotes;

        public Polls()
        {
            m_Underlying = new RegexCommandMessageProcessor(Commands);
        }

        public Dictionary<string, MessageHandler> Commands
        {
            get
            {
                return new Dictionary<string, MessageHandler>
                {
                    { "vote (?<vote>.*)", Vote },
                    { "start poll|poll start|poll open|open poll", StartPoll },
                    { "poll add (?<choice>.*)", AddChoice },
                    { "poll close|close poll|poll finish|finish poll", FinishPoll }
                };
            }
        }

        private MessageResult FinishPoll(Command message, Match args)
        {
            if (!m_PollRunning)
            {
                return ComplainAboutNoPollRunning(message);
            }
            m_PollRunning = false;
            if (!m_CurrentPollVotes.Any())
            {
                return Response.ToMessage(message, "Voter turnout is rather disappointing today. No votes were cast.");
            }
            var votesPerChoice = m_CurrentPollVotes.Aggregate(new Dictionary<int, int>(), AddVote);
            var winner = votesPerChoice.MaxBy(x => x.Value).Key;
            var winningChoice = m_CurrentPollChoices[winner - 1];
            return Response.ToMessage(message,
                string.Format("Polls closed! The winner is choice {0} ({1}).\n{2}", winner, winningChoice,
                    FormatResults(votesPerChoice, m_CurrentPollChoices)));
        }

        private string FormatResults(Dictionary<int, int> votesPerChoice, List<string> choiceStrings)
        {
            return String.Join("\n",
                votesPerChoice.OrderByDescending(x => x.Value)
                    .Select(x => "`" + new string('#', x.Value) + "` " + choiceStrings[x.Key - 1]));
        }

        private Dictionary<int, int> AddVote(Dictionary<int, int> votes, KeyValuePair<string, int> nextVote)
        {
            var voter = nextVote.Key;
            var vote = nextVote.Value;
            if (votes.ContainsKey(vote))
            {
                votes[vote]++;
            }
            else
            {
                votes[vote] = 1;
            }
            return votes;
        }

        private MessageResult AddChoice(Command message, Match args)
        {
            if (!m_PollRunning)
            {
                return ComplainAboutNoPollRunning(message);
            }
            var choice = args.Group("choice");
            m_CurrentPollChoices.Add(choice);
            return Response.ToMessage(message, 
                "Choice added. Use `vote " + m_CurrentPollChoices.Count + "` to vote for it.");
        }

        private static Response ComplainAboutNoPollRunning(Command message)
        {
            return Response.ToMessage(message, "The poll is not currently running. Use `poll start` to start a poll.");
        }

        private MessageResult StartPoll(Command message, Match args)
        {
            if (m_PollRunning)
            {
                return Response.ToMessage(message, "A poll is already running in " + m_CurrentPollChannel);
            }
            m_PollRunning = true;
            m_CurrentPollChannel = message.Channel;
            m_CurrentPollChoices = new List<string>();
            m_CurrentPollVotes = new Dictionary<string, int>();
            return Response.ToMessage(message, "Polling started. Use `poll add <some choice>` to add choices " +
                                               "and `vote 1` to vote for a particular option, then `poll finish` to show the results.");
        }

        private MessageResult Vote(Command message, Match args)
        {
            if (!m_PollRunning)
            {
                return ComplainAboutNoPollRunning(message);
            }
            int vote;
            if (!Int32.TryParse(args.Group("vote"), out vote))
            {
                return Response.ToMessage(message, "Don't know how to vote for \"" + args.Group("vote" + "\""));
            }
            if (vote > m_CurrentPollChoices.Count)
            {
                return Response.ToMessage(message,
                    "There isn't an choice " + vote + ". Use `poll add <some choice> to add more choices.");
            }
            m_CurrentPollVotes[message.User] = vote;
            return Response.ToMessage(message,
                "Vote added for choice " + vote + " (" + m_CurrentPollChoices[vote - 1] + "). You can change your vote by voting for something else.");
        }

        public MessageResult ProcessTimerTick()
        {
            return m_Underlying.ProcessTimerTick();
        }

        public MessageResult ProcessCommand(Command message)
        {
            return m_Underlying.ProcessCommand(message);
        }
    }
}

