using scbot.core.bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scbot.core.utils
{
    public interface ICommandProcessor
    {
        MessageResult ProcessTimerTick();
        MessageResult ProcessCommand(Command command);
    }
}
