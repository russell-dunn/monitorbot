using scbot.review.diffparser;
using System.Linq;

namespace scbot.review.reviewer
{
	class DontAddTabCharacters : DiffReviewerBase
    {
        public override void Visit(AddedLine line)
        {
            base.Visit(line);
            if (line.Line.Contains('\t'))
            {
                Comments.Add(new DiffComment("tab-characters", string.Format("Tab character added at line {0} in {1}", CurrentNewFileLineNumber, CurrentNewFile)));
            }
        }
    }
}
