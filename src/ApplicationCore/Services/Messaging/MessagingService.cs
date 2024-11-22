using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.eShopWeb.ApplicationCore.Services.Messaging;
internal class MessagingService
{

    ConnectionFactory factory;
    private readonly RabbitMqSettings _settings;

    string exchangeName = "";

    public MessagingService(IOptions<RabbitMqSettings> settings) 
    {
        _settings = settings.Value;
        factory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
        };
        exchangeName = "ewebshop";

    }
    //private static MessagingService _messagingService = null;

    //public static MessagingService messagingService
    //{
    //    get 
    //    {
    //        if (_messagingService == null)
    //        {
    //            _messagingService = new MessagingService();
    //        }
    //        return _messagingService;
    //    }
    //}


    // TODO: Make routingKeys enums
    public async Task SendMessage(string message, string _routingKey, string queueName)
    {

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic, durable: true);
        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: _routingKey);

       
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: exchangeName,
                             routingKey: _routingKey,
                             body: body);
        Console.WriteLine($" [x] Sent '{_routingKey}':'{message}'");
    }

    

    //public async Task ConsumerReceived(object sender, BasicDeliverEventArgs ea)
    //{
    //    var body = ea.Body.ToArray();
    //    var message = Encoding.UTF8.GetString(body);
    //    Console.WriteLine($"Received: {message}");
    //} 

}

public class MessagingServiceRecive
{
    ConnectionFactory factory;
    private readonly RabbitMqSettings _settings;
    string exchangeName = "";

    string Message = "";

    public MessagingServiceRecive(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
        factory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
        };
        exchangeName = "ewebshop";

    }
    public async Task<string> ReceiveMessage(string routingKey, string queueName)
    {
        //var factory = new ConnectionFactory { HostName = "localhost" };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();


        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
        Console.WriteLine("Waiting for messages");

        var consumer = new AsyncEventingBasicConsumer(channel);
        var latch = new AutoResetEvent(false); 
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            Message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received message: {Message}");

            latch.Set();
        };

        await channel.BasicConsumeAsync(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
        
        latch.WaitOne();
        return Message;
    
    
    
    
    
    }
}
