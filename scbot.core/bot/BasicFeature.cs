using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.bot
{
    public class BasicFeature : IFeature
    {
        private readonly string m_Description;
        private readonly string m_HelpText;
        private readonly IMessageProcessor m_MessageProcessor;
        private readonly string m_Name;

        public BasicFeature(string name, string description, string helpText, IMessageProcessor messageProcessor)
        {
            m_Name = name;
            m_Description = description;
            m_HelpText = helpText;
            m_MessageProcessor = messageProcessor;
        }

        public string Description { get { return m_Description; } }

        public string HelpText { get { return m_HelpText; } }

        public IMessageProcessor MessageProcessor { get { return m_MessageProcessor; } }

        public string Name { get { return m_Name; } }
    }
}
