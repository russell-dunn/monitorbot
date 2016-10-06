using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monitorbot.review.diffparser;

namespace monitorbot.review.reviewer
{
    class MaximumLineLength : DiffReviewerBase
    {
        private readonly int m_Chars;

        public MaximumLineLength(int chars)
        {
            m_Chars = chars;
        }

        public override void Visit(AddedLine line)
        {
            base.Visit(line);
            if (line.Line.Length > m_Chars)
            {
                Comments.Add(new DiffComment("Long line found (>" + m_Chars + " chars)", CurrentNewFile, CurrentNewFileLineNumber));
            }
        }
    }
}
