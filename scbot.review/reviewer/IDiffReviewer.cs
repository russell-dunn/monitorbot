using scbot.review.diffparser;
using System.Collections.Generic;

namespace scbot.review.reviewer
{
	// TODO: not really sure about this interface..
	interface IDiffReviewer : IDiffLineVisitor
    {
        List<DiffComment> Comments { get; }
    }
}
