namespace scbot.review.reviewer
{
    public class DiffComment
    {
        public readonly string Description;
        public readonly string File;
        public readonly int Line;

        public DiffComment(string description, string file, int line)
        {
            Description = description;
            File = file;
            Line = line;
        }
    }
}