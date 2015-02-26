namespace scbot.core.bot
{
    public interface IMessageProcessor
    {
        MessageResult ProcessTimerTick();
        MessageResult ProcessMessage(Message message);
    }
}