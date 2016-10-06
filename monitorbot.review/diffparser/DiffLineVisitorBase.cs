namespace monitorbot.review.diffparser
{
    class DiffLineVisitorBase : IDiffLineVisitor
    {
        public virtual void Visit(OldFile file) { }

        public virtual void Visit(ContextLine line) { }

        public virtual void Visit(RemovedLine line) { }

        public virtual void Visit(AddedLine line) { }

        public virtual void Visit(NewFile file) { }

        public virtual void Visit(ChunkHeader header) { }

        public virtual void Visit(GitDiffHeader header) { }
    }
}
