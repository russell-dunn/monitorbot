namespace scbot.slack
{
    public interface IBot
    {
        MessageResult Hello();
        MessageResult UnknownMessage(string json);
    }
}