using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using scbot.review.reviewer;
using scbot.review.diffparser;

namespace scbot.review.services
{
	class ReviewApi : IReviewApi
	{
		private readonly IGithubDiffApi m_Github;
		private readonly IEnumerable<Func<IDiffReviewer>> m_Reviewers;

		public ReviewApi(IGithubDiffApi github, IEnumerable<Func<IDiffReviewer>> reviewers)
		{
			m_Github = github;
			m_Reviewers = reviewers.ToList();
		}

		private IEnumerable<DiffComment> Review(string diff)
		{
			var parsedDiff = DiffParser.ParseDiff(diff);
			foreach (var reviewerCreator in m_Reviewers)
			{
				var reviewer = reviewerCreator();
				foreach (var comment in reviewer.Review(parsedDiff))
				{
					yield return comment;
				}
			}
		}
				


		public async Task<IEnumerable<DiffComment>> ReviewForCommit(string user, string repo, string hash)
		{
			return Review(await m_Github.DiffForCommit(user, repo, hash));
		}

		public async Task<IEnumerable<DiffComment>> ReviewForComparison(string user, string repo, string comparison)
		{
			// TODO: also review each commit in comparison for commit-specific stuff?
			return Review(await m_Github.DiffForComparison(user, repo, comparison));
		}

		public async Task<IEnumerable<DiffComment>> ReviewForPullRequest(string user, string repo, int pr)
		{
			// TODO: also review each commit in comparison for commit-specific stuff?
			return Review(await m_Github.DiffForPullRequest(user, repo, pr));
		}
	}
}
