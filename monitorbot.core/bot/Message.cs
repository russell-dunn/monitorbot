namespace monitorbot.core.bot
{
    public struct Message
    {
        public readonly string Channel;
        public readonly string User;
        public readonly string MessageText;

        public Message(string channel, string user, string messageText)
        {
            Channel = channel;
            User = user;
            MessageText = messageText;
        }
    }
}