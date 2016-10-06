using System.Collections.Generic;
using monitorbot.review.diffparser;

namespace monitorbot.review.reviewer
{
    abstract class DiffReviewerBase : LineVisitorContext, IDiffReviewer
    {
        private readonly List<DiffComment> m_Comments = new List<DiffComment>();

        public List<DiffComment> Comments { get { return m_Comments; } }
    }
}
