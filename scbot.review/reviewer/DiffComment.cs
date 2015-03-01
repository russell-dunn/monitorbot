namespace scbot.review.reviewer
{
    public class DiffComment
    {
        public readonly string Description;
        public readonly string Type;
        public readonly string File;
        public readonly int Line;

        public DiffComment(string type, string description, string file, int line)
        {
            Type = type;
            Description = description;
            File = file;
            Line = line;
        }
    }
}