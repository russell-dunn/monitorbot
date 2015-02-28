namespace scbot.review.reviewer
{
    public class DiffComment
    {
        private readonly string Description;
        private readonly string Type;

        public DiffComment(string type, string description)
        {
            Type = type;
            Description = description;
        }
    }
}