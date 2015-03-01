using scbot.review.diffparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.reviewer
{
    class UseWindowsNewlines : DiffReviewerBase
    {
        public override void Visit(AddedLine line)
        {
            base.Visit(line);
            // hack: we split on '\n' so we expect \r to be left over
            if (!line.Line.EndsWith("\r"))
            {
                Comments.Add(new DiffComment("Unix newline found", CurrentNewFile, CurrentNewFileLineNumber));
            }
        }
    }
}
