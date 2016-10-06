namespace monitorbot.review.diffparser
{
    public abstract class DiffLine
    {
        private readonly string m_Line;

        public string Line { get { return m_Line; } }

        protected DiffLine(string line)
        {
            m_Line = line;
        }

        public override string ToString()
        {
            return "<" + this.GetType().Name + " " + Line + ">";
        }

        public abstract void Accept(IDiffLineVisitor visitor);
    }

    public class GitDiffHeader : DiffLine
    {
        public GitDiffHeader(string line) : base(line)
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
    
    public class ChunkHeader : DiffLine
    {
        public ChunkHeader(string line) : base(line.Substring(3))
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class OldFile : DiffLine
    {
        public OldFile(string line) : base(line.Substring(6))
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class NewFile : DiffLine
    {
        public NewFile(string line) : base(line.Substring(6))
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class ContextLine : DiffLine
    {
        public ContextLine(string line) : base(line.Substring(1))
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class AddedLine : DiffLine
    {
        public AddedLine(string line) : base(line.Substring(1))
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public class RemovedLine : DiffLine
    {
        public RemovedLine(string line) : base(line.Substring(1))
        {
        }

        public override void Accept(IDiffLineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
