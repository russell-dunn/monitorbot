namespace scbot.services
{
    public struct Note
    {
        public readonly string Id;
        public readonly string Text;

        public Note(string id, string text)
        {
            Id = id;
            Text = text;
        }
    }
}