namespace scbot.services
{
    public struct ZendeskTicket
    {
        public readonly string Id;
        public readonly string Description;
        public readonly string Status;
        public readonly int CommentCount;

        public ZendeskTicket(string id, string description, string status, int commentCount)
        {
            Id = id;
            Description = description;
            Status = status;
            CommentCount = commentCount;
        }
    }
}