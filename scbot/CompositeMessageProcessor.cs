﻿using System.Collections.Generic;
using System.Linq;

namespace scbot
{
    public class CompositeMessageProcessor : IMessageProcessor
    {
        private readonly IEnumerable<IMessageProcessor> m_SubProcessors;

        public CompositeMessageProcessor(params IMessageProcessor[] subProcessors)
        {
            m_SubProcessors = subProcessors;
        }

        public MessageResult ProcessTimerTick()
        {
            return MessageResult.Empty;
        }

        public MessageResult ProcessMessage(Message message)
        {
            return new MessageResult(m_SubProcessors.SelectMany(x => x.ProcessMessage(message).Responses));
        }
    }
}