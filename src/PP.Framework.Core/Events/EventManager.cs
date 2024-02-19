using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenTracing;
using PaulPhillips.Framework.Feature.Events.Contracts;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PaulPhillips.Framework.Feature.Events;

public class EventManager(IConfiguration configuration, ITracer tracer) : IEventManager
{
    public bool SendEvent<T>(string exchange, string routingKey, string queueName, T eventData)
    {
        using var channel = CreateChannel(queueName);
        if (channel == null)
        {
            return true;
        }

        channel.BasicPublish(exchange: exchange,
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(eventData)));

        return true;
    }

    public void ReceivedEvent(string queueName, Func<dynamic, bool> delMessage)
    {
        var requestSpan = tracer.BuildSpan(queueName).StartActive(true).Span;

        var channel = CreateChannel(queueName);
        if (channel != null)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var result = delMessage(message);
                if (result)
                {
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                else
                {
                    channel.BasicReject(ea.DeliveryTag,false);
                }
            };

            channel.BasicConsume(queue: queueName,
                     autoAck: false,
                     consumer: consumer);



        }

    }

    private IModel? CreateChannel(string queueName)
    {
        var factory = new ConnectionFactory { HostName = configuration["Events:Host"], UserName = configuration["Events:UserName"], Password = configuration["Events:Password"] };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.QueueDeclare(queue: queueName,
                     durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

        return channel;
    }


}
