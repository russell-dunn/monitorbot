namespace scbot
{
    public class Response
    {
        public readonly string Message;
        public readonly string Channel;

        public Response(string message, string channel)
        {
            Message = message;
            Channel = channel;
        }

        public static Response ToMessage(Message message, string text)
        {
            return new Response(text, message.Channel);
        }
    }
}