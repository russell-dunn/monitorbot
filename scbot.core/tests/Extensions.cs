using Moq;
using scbot.core.bot;

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
