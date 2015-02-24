namespace scbot.bot
{
    public interface ICommandParser
    {
        /// <summary>
        /// Figure out if a message is addressed to the bot
        /// </summary>
        /// <returns>true iff <see cref="command"/> contains a command that the bot should respond to</returns>
        bool TryGetCommand(Message message, out string command);
    }
}