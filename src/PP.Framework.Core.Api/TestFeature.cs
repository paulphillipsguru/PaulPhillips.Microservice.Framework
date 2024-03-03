using OpenTracing;
using PaulPhillips.Framework.Feature.Commands;
using PaulPhillips.Framework.Feature.Events;

namespace PP.Framework.Core.Api
{
    public class TestFeature: Command<Request, bool>
    {
        public override async Task<dynamic> ProcessAsync(ISpan tracingSpan)
        {
            return await Task.FromResult("Hello");
        }

        public override void Compensate(ISpan tracingSpan)
        {
            base.Compensate(tracingSpan);
        }

    }

    public class HandleCompensation : BaseEvent<string>
    {
        public override bool Process(ISpan tracingSpan)
        {
            return true;
        }
    }

    public class Request
    {
        public string Name { get; set; }
    }
}
