using System;
using System.Linq;
using monitorbot.core.bot;
using monitorbot.core.utils;

namespace monitorbot.core.meta
{
    public class ConcattingMessageProcessor : IMessageProcessor
    {
        private readonly IMessageProcessor m_Underlying;

        public ConcattingMessageProcessor(IMessageProcessor underlying)
        {
            m_Underlying = underlying;
        }

        public MessageResult ProcessTimerTick()
        {
            var before = m_Underlying.ProcessTimerTick();
            return new MessageResult(before.Responses.GroupBy(Channel).Select(CreateResponse).ToList());
        }

        public MessageResult ProcessMessage(Message message)
        {
            var before = m_Underlying.ProcessMessage(message);
            return new MessageResult(before.Responses.GroupBy(Channel).Select(CreateResponse).ToList());
        }

        private string Channel(Response x)
        {
            return x.Channel;
        }

        private Response CreateResponse(IGrouping<string, Response> grouping)
        {
            var messages = grouping.Select(x => x.Message);
            var image = grouping.Select(x => x.Image).FirstOrDefault(x => x.IsNotDefault());
            var channel = grouping.Key;
            return new Response(String.Join("\n", messages), channel, image);
        }
    }
}