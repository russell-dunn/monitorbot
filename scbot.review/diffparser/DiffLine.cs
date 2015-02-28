using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.review.diffparser
{
    public abstract class DiffLine
    {
        protected readonly string m_Line;
        protected DiffLine(string line)
        {
            m_Line = line;
        }

        public override string ToString()
        {
            return "<" + this.GetType().Name + " " + m_Line + ">";
        }
    }

    public class GitDiffHeader : DiffLine { public GitDiffHeader(string line) : base(line) { } }
    public class ChunkHeader : DiffLine { public ChunkHeader(string line) : base(line.Substring(3)) { } }
    public class OldFile : DiffLine { public OldFile(string line) : base(line.Substring(4)) { } }
    public class NewFile : DiffLine { public NewFile(string line) : base(line.Substring(4)) { } }
    public class ContextLine : DiffLine { public ContextLine(string line) : base(line.Substring(1)) { } }
    public class AddedLine : DiffLine { public AddedLine(string line) : base(line.Substring(1)) { } }
    public class RemovedLine : DiffLine { public RemovedLine(string line) : base(line.Substring(1)) { } }
}
