namespace scbot
{
    public struct Message
    {
        public readonly string Channel;
        public readonly string Username;
        public readonly string MessageText;

        public Message(string channel, string username, string messageText)
        {
            Channel = channel;
            Username = username;
            MessageText = messageText;
        }
    }
}