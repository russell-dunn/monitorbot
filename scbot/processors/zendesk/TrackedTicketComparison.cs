using scbot.services;

namespace scbot.processors
{
    internal class TrackedTicketComparison
    {
        public readonly string Channel;
        public readonly string Id;
        public readonly ZendeskTicket OldValue;
        public readonly ZendeskTicket NewValue;

        public TrackedTicketComparison(string channel, string id, ZendeskTicket oldValue, ZendeskTicket newValue)
        {
            Channel = channel;
            Id = id;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}