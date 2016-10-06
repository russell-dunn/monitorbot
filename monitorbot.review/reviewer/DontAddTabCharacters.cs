using System.Linq;
using monitorbot.review.diffparser;

namespace monitorbot.review.reviewer
{
    class DontAddTabCharacters : DiffReviewerBase
    {
        public override void Visit(AddedLine line)
        {
            base.Visit(line);
            if (line.Line.Contains('\t'))
            {
                Comments.Add(new DiffComment("Tab character found", CurrentNewFile, CurrentNewFileLineNumber));
            }
        }
    }
}
