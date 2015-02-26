using Moq;
using scbot.core.bot;

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
