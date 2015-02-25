namespace scbot.services.compareengine
{
    public class Update<T>
    {
        public readonly string Channel; // TODO: this is probably the wrong place to store Channel

        public readonly T OldValue;
        public readonly T NewValue;

        public Update(string channel, T oldValue, T newValue)
        {
            Channel = channel;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
