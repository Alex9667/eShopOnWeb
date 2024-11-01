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
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.eShopWeb.ApplicationCore.Services.Messaging;
internal class MessagingService
{

    ConnectionFactory factory;

    string exchangeName = "";

    public MessagingService() 
    { 
        factory = new ConnectionFactory
        {
            HostName = "localhost"
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
    public void SendMessage(string message, string _routingKey)
    {

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

       
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: exchangeName,
                             routingKey: _routingKey,
                             basicProperties: null,
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

    string exchangeName = "";

    string Message = "";

    public MessagingServiceRecive()
    {
        factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        exchangeName = "ewebshop";

    }
    public async Task<string> ReceiveMessage(string routingKey, string queueName)
    {
        var factory = new ConnectionFactory { HostName = "localhost", DispatchConsumersAsync = true };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();


        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        Console.WriteLine("Waiting for messages");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            Message = Encoding.UTF8.GetString(body);
            
            
        };

        channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
        Thread.Sleep(1000);
        return Message;
    }
}
