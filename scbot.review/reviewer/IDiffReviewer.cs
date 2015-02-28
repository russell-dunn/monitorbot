using scbot.review.diffparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.reviewer
{
    // TODO: not really sure about this interface..
    interface IDiffReviewer : IDiffLineVisitor
    {
        List<DiffComment> Comments { get; }
    }
}
