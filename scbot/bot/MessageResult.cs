using System.Collections.Generic;
using System.Linq;

namespace scbot
{
    public class MessageResult
    {
        public readonly IEnumerable<Response> Responses;

        public MessageResult(IEnumerable<Response> responses)
        {
            Responses = responses;
        }

        public static readonly MessageResult Empty = new MessageResult(Enumerable.Empty<Response>());
    }
}