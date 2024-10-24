﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microsoft.eShopWeb.ApplicationCore.Services.Messaging;
internal class MessagingService
{

    ConnectionFactory factory;

    string exchangeName = "";

    MessagingService() 
    { 
        factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        exchangeName = "ewebshop";

    }
    private static MessagingService _messagingService = null;

    public static MessagingService messagingService
    {
        get 
        {
            if (_messagingService == null)
            {
                _messagingService = new MessagingService();
            }
            return _messagingService;
        }
    }



    public void SendMessage(string message, string _routingKey)
    {

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _messagingService.exchangeName, type: ExchangeType.Topic, durable: true);

       
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: _messagingService.exchangeName,
                             routingKey: _routingKey,
                             basicProperties: null,
                             body: body);
        Console.WriteLine($" [x] Sent '{_routingKey}':'{message}'");
    }

    public static async Task ReceiveMessage()
    {
        var factory = new ConnectionFactory { HostName = "localhost", DispatchConsumersAsync=true };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        
        channel.ExchangeDeclare(exchange: _messagingService.exchangeName, type: ExchangeType.Topic, durable: true);

        var queueName = "ewebshop";

        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        channel.QueueBind(queue: queueName, exchange: _messagingService.exchangeName, routingKey: "ewebshop");

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        Console.WriteLine("Waiting for messages");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += ConsumerReceived; 

        channel.BasicConsume(queue: "ewebshop",
                             autoAck: true,
                             consumer: consumer);
    }

    public static async Task ConsumerReceived(object sender, BasicDeliverEventArgs ea)
    {
                
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Received: {message}");
        
    } 

}

