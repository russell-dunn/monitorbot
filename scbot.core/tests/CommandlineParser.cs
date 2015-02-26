using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using scbot.bot;

namespace scbot.core.tests
{
    public static class CommandlineParser
    {
        public static ICommandParser For(string commandToReturn)
        {
            var mock = new Mock<ICommandParser>();
            mock.SetupTryGetCommand(commandToReturn);
            return mock.Object;
        }
    }
}
