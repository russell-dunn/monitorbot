namespace scbot.services
{
    public class JiraBug
    {
        public readonly string Id;
        public readonly string Title;
        public readonly string Status;
        public readonly int CommentCount;

        public JiraBug(string id, string title, string status, int commentCount)
        {
            Id = id;
            Title = title;
            Status = status;
            CommentCount = commentCount;
        }
    }
}