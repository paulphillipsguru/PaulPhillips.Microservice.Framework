using Microsoft.AspNetCore.Http;
using OpenTracing;
using PaulPhillips.Framework.Feature.Commands.Contracts;
using PaulPhillips.Framework.Feature.Core;
using PaulPhillips.Framework.Feature.Core.Contracts;
using PaulPhillips.Framework.Feature.Events.Contracts;
using PaulPhillips.Framework.Feature.Idempotency.Contracts;


namespace PaulPhillips.Framework.Feature.Middlewares
{
    public class FeaterCoreMiddleware
    {
        private readonly FeatureService _featureService;
        private readonly ITracer _tracer;
        private readonly IEventManager _eventManager;
        private readonly IIdempotency _IIdempotency;
        private readonly RequestDelegate _next;


        public FeaterCoreMiddleware(RequestDelegate next, FeatureService featureService,
            ITracer tracer, IEventManager eventManager, IIdempotency Idempotency, ISagaSupport sageSupport)
        {
            _featureService = featureService;
            _tracer = tracer;
            _eventManager = eventManager;
            _IIdempotency = Idempotency;
            _next = next;

            sageSupport.WatchForSagaEvents();

            foreach (var eventFeature in FeatureFactory.Events)
            {
                if (Activator.CreateInstance(eventFeature.Value) is IEvent eventHandler)
                {
                    eventManager.ReceivedEvent(eventFeature.Key, (data) =>
                    {
                        var requestSpan = tracer.BuildSpan(eventFeature.Key).StartActive(true).Span;
                        eventHandler.LoadData(data);

                        return eventHandler.Process(requestSpan);

                    });
                }
            }


        }

        public async Task Invoke(HttpContext context)
        {
            if (_featureService != null)
            {
                await FeatureService.RequestHandler(context, _tracer, _eventManager, _IIdempotency);
            }

            await context.Response.CompleteAsync();
            await _next(context);
        }
    }
}

