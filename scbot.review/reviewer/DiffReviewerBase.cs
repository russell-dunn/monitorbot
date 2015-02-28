using scbot.review.diffparser;
using System.Collections.Generic;

namespace scbot.review.reviewer
{
	abstract class DiffReviewerBase : LineVisitorContext, IDiffReviewer
    {
        private readonly List<DiffComment> m_Comments = new List<DiffComment>();

        public List<DiffComment> Comments { get { return m_Comments; } }
    }
}
