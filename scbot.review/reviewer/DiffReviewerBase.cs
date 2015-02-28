using scbot.review.diffparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.reviewer
{
    abstract class DiffReviewerBase : LineVisitorContext, IDiffReviewer
    {
        private readonly List<DiffComment> m_Comments = new List<DiffComment>();

        public List<DiffComment> Comments { get { return m_Comments; } }
    }
}
