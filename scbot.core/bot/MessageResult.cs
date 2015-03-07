using System.Collections.Generic;
using System.Linq;

namespace scbot.core.bot
{
    public class MessageResult
    {
        public readonly IEnumerable<Response> Responses;

        public MessageResult(IEnumerable<Response> responses)
        {
            Responses = responses;
        }

        public MessageResult(Response response) : this(new[] { response }) { }

        public static implicit operator MessageResult(Response response) { return new MessageResult(response); }

        public static readonly MessageResult Empty = new MessageResult(Enumerable.Empty<Response>());
    }
}