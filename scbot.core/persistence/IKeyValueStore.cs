namespace scbot.core.persistence
{
    public interface IKeyValueStore
    {
        void Set(string key, string value);
        string Get(string key);
        void Clear();
    }
}