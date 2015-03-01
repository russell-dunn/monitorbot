namespace scbot.review.reviewer
{
    public class DiffComment
    {
        public readonly string Description;
        public readonly string Type;

        public DiffComment(string type, string description)
        {
            Type = type;
            Description = description;
        }
    }
}