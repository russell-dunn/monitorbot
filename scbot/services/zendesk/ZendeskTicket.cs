using System.Collections.Generic;
namespace scbot.services
{
    public struct ZendeskTicket
    {
        public struct Comment
        {
            public readonly string Text;
            public readonly string Author;
            public readonly string Avatar;

            public Comment(string text, string author, string avatar)
            {
                Text = text;
                Author = author;
                Avatar = avatar;
            }
        }

        public readonly string Id;
        public readonly string Description;
        public readonly string Status;
        public readonly IReadOnlyCollection<Comment> Comments;

        public ZendeskTicket(string id, string description, string status, IReadOnlyCollection<Comment> comments)
        {
            Id = id;
            Description = description;
            Status = status;
            Comments = comments;
        }
    }
}