using scbot.core.meta;
using scbot.core.utils;
using System;
using static System.String;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.bot
{
    public class FeatureMessageProcessor : IMessageProcessor
    {
        private readonly ICommandParser m_CommandParser;
        private Dictionary<string, IFeature> m_Features;
        private readonly CompositeMessageProcessor m_MessageProcessor;

        public FeatureMessageProcessor(ICommandParser commandParser, params IFeature[] features)
        {
            m_CommandParser = commandParser;
            m_Features = GetFeaturesByName(features);
            m_MessageProcessor = new CompositeMessageProcessor(features.Select(x => x.MessageProcessor).ToArray());
        }

        private Dictionary<string, IFeature> GetFeaturesByName(IFeature[] features)
        {
            return features.ToDictionary(feature => feature.Name.ToLowerInvariant(), feature => feature);
        }

        public MessageResult ProcessMessage(Message message)
        {
            string command;
            if (m_CommandParser.TryGetCommand(message, "help", out command))
            {
                if (!IsNullOrWhiteSpace(command))
                {
                    command = command.ToLowerInvariant();
                    if (m_Features.ContainsKey(command))
                    {
                        var feature = m_Features[command];
                        return Response.ToMessage(message, Format("{0}\n{1}", FormatFeature(feature), feature.HelpText));
                    }
                    else
                    {
                        return Response.ToMessage(message, Format("Sorry, I don't know about '{0}'.", command));
                    }
                }

                return Response.ToMessage(message, Join("\n", m_Features.Values.Select(FormatFeature)));
            }
            return m_MessageProcessor.ProcessMessage(message);
        }

        private string FormatFeature(IFeature feature)
        {
            return Format("*{0}*: {1}", feature.Name, feature.Description);
        }

        public MessageResult ProcessTimerTick()
        {
            return m_MessageProcessor.ProcessTimerTick();
        }
    }
}
