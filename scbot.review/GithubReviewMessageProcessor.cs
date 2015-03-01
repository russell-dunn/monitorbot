using System;
using scbot.core.bot;
using scbot.review.services;
using scbot.core.utils;
using scbot.review.reviewer;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace scbot.review
{
	internal class GithubReviewMessageProcessor : IMessageProcessor
	{
		private readonly ICommandParser m_CommandParser;
		private readonly string m_DefaultRepo;
		private readonly string m_DefaultUser;
		private readonly IReviewApi m_ReviewApi;

		public GithubReviewMessageProcessor(ICommandParser commandParser, IReviewApi review, string defaultUser = null, string defaultRepo = null)
		{
			m_CommandParser = commandParser;
			m_ReviewApi = review;
			m_DefaultUser = defaultUser;
			m_DefaultRepo = defaultRepo;
		}

		public MessageResult ProcessMessage(Message message)
		{
			string toReview;
			if (m_CommandParser.TryGetCommand(message, "review", out toReview))
			{
				int prNumber;
				if (int.TryParse(toReview.Trim(), out prNumber))
				{
					return FormatComments(message, m_ReviewApi.ReviewForPullRequest(m_DefaultUser, m_DefaultRepo, prNumber).Result);
				}
				else
				{
					return FormatComments(message, m_ReviewApi.ReviewForCommit(m_DefaultUser, m_DefaultRepo, toReview.Trim()).Result);
				}
			}
			return MessageResult.Empty;
		}

		private MessageResult FormatComments(Message message, IEnumerable<DiffComment> comments)
		{
			if (!comments.Any())
			{
				return new MessageResult(new[] { Response.ToMessage(message, "No issues detected. Looks good!") });
			}
			var responses = comments.Distinct().Take(20).Select(x => Response.ToMessage(message, x.Description));
			return new MessageResult(responses);
		}

		public MessageResult ProcessTimerTick()
		{
			return MessageResult.Empty;
		}
	}
}