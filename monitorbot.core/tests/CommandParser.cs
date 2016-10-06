using monitorbot.core.bot;
using Moq;

namespace monitorbot.core.tests
{
    public static class CommandParser
    {
        public static ICommandParser For(string commandToReturn)
        {
            var mock = new Mock<ICommandParser>();
            mock.SetupTryGetCommand(commandToReturn);
            return mock.Object;
        }
    }
}
