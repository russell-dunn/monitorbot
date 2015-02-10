using System;
using System.Linq;

namespace scbot.processors
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
            return MessageResult.Empty;
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
            var channel = grouping.Key;
            return new Response(String.Join("\n", messages), channel);
        }
    }
}