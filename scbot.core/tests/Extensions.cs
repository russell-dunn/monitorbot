using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using scbot.bot;

namespace scbot.core.tests
{
    public static class Extensions
    {
        public static void SetupTryGetCommand(this Mock<ICommandParser> commandParser, string commandText)
        {
            var command = commandText;
            commandParser.Setup(x => x.TryGetCommand(It.IsAny<Message>(), out command)).Returns(true);
        }
    }
}
