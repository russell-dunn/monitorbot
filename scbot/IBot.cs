namespace scbot
{
    public interface IBot
    {
        MessageResult Hello();
        MessageResult Unknown(string json);
        MessageResult Message(Message message);
        MessageResult TimerTick();
    }
}