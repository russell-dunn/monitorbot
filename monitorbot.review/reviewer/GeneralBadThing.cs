using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using monitorbot.review.diffparser;

namespace monitorbot.review.reviewer
{
    class GeneralBadThing : DiffReviewerBase
    {
        private readonly string m_Message;
        private readonly Regex m_Regex;

        public GeneralBadThing(string regex, string message, RegexOptions extraOptions = RegexOptions.None)
        {
            m_Regex = new Regex(regex, RegexOptions.Compiled | extraOptions);
            m_Message = message;
        }

        public override void Visit(AddedLine line)
        {
            base.Visit(line);
            if (m_Regex.IsMatch(line.Line))
            {
                Comments.Add(new DiffComment(m_Message, CurrentNewFile, CurrentNewFileLineNumber));
            }
        }
    }
}
