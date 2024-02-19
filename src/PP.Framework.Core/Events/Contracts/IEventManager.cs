namespace PaulPhillips.Framework.Feature.Events.Contracts;
public interface IEventManager
{
    bool SendEvent<T>(string exchange, string routingKey, string queueName, T eventData);
    void ReceivedEvent(string queueName, Func<dynamic, bool> messageDel);
}
