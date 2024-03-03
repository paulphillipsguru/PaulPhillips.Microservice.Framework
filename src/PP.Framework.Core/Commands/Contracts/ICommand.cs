using AutoMapper;
using OpenTracing;
using PaulPhillips.Framework.Feature.Core.Contracts;
using PaulPhillips.Framework.Feature.Validation.Contracts;

namespace PaulPhillips.Framework.Feature.Commands.Contracts;
public interface ICommand : IFeatureCore
{
    void Compensate(ISpan tracingSpan);
    IModelValidation? GetValidation();
    IModelValidation? GetEntityValidation();
    void RequestValidationFailed();
    void EntityValidationFailed();
    void SetData(dynamic data);
    void SetupMapping(MapperConfigurationExpression expression);
    void MapToEntity(IMapper mapper, ISpan tracingSpan);
    bool DomainEvent(string exchange, string routingKey, string queueName, string eventName);
}
