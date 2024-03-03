using Jaeger;
using OpenTracing;
using PaulPhillips.Framework.Feature.Commands.Contracts;
using PaulPhillips.Framework.Feature.Core.Contracts;
using PaulPhillips.Framework.Feature.Events.Contracts;

namespace PaulPhillips.Framework.Feature.Core
{
    internal class SagaSupport(IEventManager eventManager, ITracer tracer) : ISagaSupport
    {
        public void WatchForSagaEvents()
        {
            foreach (var eventFeature in FeatureFactory.Features)
            {
                if (Activator.CreateInstance(eventFeature.Value) is ICommand command)
                {
                    eventManager.ReceivedEvent(eventFeature.Key + "_SAGA", (data) =>
                    {
                        var requestSpan = tracer.BuildSpan(eventFeature.Key).StartActive(true).Span;

                        command.SetData(data);
                        

                        command.Compensate(requestSpan);

                        return true;
                    });
                }
            }
        }
    }
}
