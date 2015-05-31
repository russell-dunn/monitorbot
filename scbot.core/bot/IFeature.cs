using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.bot
{
    public interface IFeature
    {
        /// <summary>
        /// A short name for the feature
        /// </summary>
        string Name { get; }
        /// <summary>
        /// A short (one-line) description of the feature
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The handler for how the feature is implemented
        /// </summary>
        IMessageProcessor MessageProcessor { get; }
        /// <summary>
        /// Some more detailed (multi-line) help for how to use the feature
        /// </summary>
        string HelpText { get; }
    }
}
