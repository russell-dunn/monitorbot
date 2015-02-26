namespace scbot.bot
{
    public class Response
    {
        public readonly string Message;
        public readonly string Channel;
        public readonly string Image;

        public Response(string message, string channel, string image=null)
        {
            Message = message;
            Channel = channel;
            Image = image;
        }

        public static Response ToMessage(Message message, string text, string image=null)
        {
            return new Response(text, message.Channel, image);
        }
    }
}