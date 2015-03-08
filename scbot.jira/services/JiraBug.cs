namespace scbot.jira.services
{
    public class JiraBug
    {
        public readonly string Id;
        public readonly string Type;
        public readonly string Title;
        public readonly string Status;
        public readonly int CommentCount;
        internal readonly string Description;

        public JiraBug(string id, string type, string title, string description, string status, int commentCount)
        {
            Id = id;
            Type = type;
            Title = title;
            Description = description;
            Status = status;
            CommentCount = commentCount;
        }
    }
}