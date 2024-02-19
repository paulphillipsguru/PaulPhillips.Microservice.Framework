using AutoMapper;
using Newtonsoft.Json;
using OpenTracing;
using PaulPhillips.Framework.Feature.Commands.Contracts;
using PaulPhillips.Framework.Feature.Events.Contracts;
using PaulPhillips.Framework.Feature.Validation.Contracts;
using System.Security.Claims;

namespace PaulPhillips.Framework.Feature.Commands;

public abstract class Command<R, E> : ICommand
{
    public IEventManager? EventManager { get; set; }

    public R? Request { get; set; }
    public E? Entity { get; set; }

    public virtual void RequestValidationFailed() { }
    public virtual void EntityValidationFailed() { }
    public virtual void LoadIocServices(ISpan tracingSpan) { }
    public virtual async Task LoadData(ISpan tracingSpan) { await Task.CompletedTask; }

    public void SetData(dynamic data)
    {
        Request = JsonConvert.DeserializeObject<R>(data);
    }

    public IModelValidation? GetValidation()
    {
        return Request as IModelValidation;
    }

    public IModelValidation? GetEntityValidation()
    {
        return Entity as IModelValidation;
    }

    public abstract Task<dynamic> ProcessAsync(ISpan tracingSpan);

    public virtual void SetupMapping(MapperConfigurationExpression expression)
    {
        expression.CreateMap<R, E>();
    }
    public void MapToEntity(IMapper mapper, ISpan tracingSpan)
    {
        Entity = mapper.Map<E>(Request);
    }
    public virtual bool ValidateClaims(IList<Claim> claims, ISpan tracingSpan)
    {
        return true;
    }
    public bool DomainEvent(string exchange, string routingKey, string queueName, string eventName)
    {
        if (EventManager != null)
        {
            return EventManager.SendEvent(exchange, routingKey, queueName, eventName);
        }
        else
        {
            return false;
        }
    }
}
