using System.Collections.Generic;
using monitorbot.review.diffparser;

namespace monitorbot.review.reviewer
{
    // TODO: not really sure about this interface..
    interface IDiffReviewer : IDiffLineVisitor
    {
        List<DiffComment> Comments { get; }
    }
}
