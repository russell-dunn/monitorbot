namespace scbot
{
    public class Message
    {
        public readonly string Username;
        public readonly string MessageText;

        public Message(string username, string messageText)
        {
            Username = username;
            MessageText = messageText;
        }
    }
}