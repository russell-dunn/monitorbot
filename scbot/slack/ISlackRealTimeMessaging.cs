using System;
using System.Threading;
using System.Threading.Tasks;

namespace scbot.slack
{
    public interface ISlackRealTimeMessaging : IDisposable
    {
        Task<string> Receive(CancellationToken cancellationToken);
        string BotId { get; }
    }
}