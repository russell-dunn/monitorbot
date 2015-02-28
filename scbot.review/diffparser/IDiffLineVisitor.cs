namespace scbot.review.diffparser
{
	public interface IDiffLineVisitor
    {
        void Visit(GitDiffHeader header);
        void Visit(ChunkHeader header);
        void Visit(OldFile file);
        void Visit(NewFile file);
        void Visit(ContextLine line);
        void Visit(AddedLine line);
        void Visit(RemovedLine line);
    }
}
