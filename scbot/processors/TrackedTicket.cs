using scbot.services;

namespace scbot.processors
{
    internal struct TrackedTicket
    {
        public readonly ZendeskTicket Ticket;
        public readonly string Channel;

        public TrackedTicket(ZendeskTicket ticket, string channel)
        {
            Ticket = ticket;
            Channel = channel;
        }
    }
}