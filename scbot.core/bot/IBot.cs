namespace scbot.core.bot
{
    public interface IBot
    {
        MessageResult Hello();
        MessageResult Unknown(string json);
        MessageResult Message(Message message);
        MessageResult ChannelCreated(string newChannelId, string newChannelName, string creatorId);
        MessageResult TimerTick();
    }
}