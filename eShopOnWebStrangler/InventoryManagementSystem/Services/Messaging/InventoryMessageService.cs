using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Messaging;

internal class InventoryMessageService
{
    ConnectionFactory factory;

    private InventoryDbContext context;

    string exchangeName = "";

    public InventoryMessageService(InventoryDbContext context)
    {
        factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        exchangeName = "ewebshop";

        this.context = context;
    }

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

    //TODO: add cancelation token
    public async Task ReceiveMessage(string routingKey, string queueName = "inventoryRequestQueue")
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
        var latch = new AutoResetEvent(false);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var Message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Received: {Message}");

            ProcessMessage(Message);

            latch.Set();
        };
        //ConsumerReceived;

        channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
    }

    public async Task ConsumerReceived(object sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        MessageObject[] messageObjects;
        List<InventoryModel> units = new();
        try
        {
            messageObjects = JsonSerializer.Deserialize<MessageObject[]>(message);

            foreach(var messageObject in messageObjects)
            {
                units.Add(context.Inventories.FirstOrDefault(i => i.ItemId == messageObject.ItemId));
            }

            var answer = JsonSerializer.Serialize(units);

            SendMessage(answer, "catalog");
        }
        catch (JsonException ex)
        {
            //TODO: send to invalid message queue
        }

        Console.WriteLine($"Received: {message}");
    }

    private async Task ProcessMessage(string message)
    {
        MessageObject[] messageObjects;
        List<InventoryModel> units = new();
        try
        {
            messageObjects = JsonSerializer.Deserialize<MessageObject[]>(message);

            foreach (var messageObject in messageObjects)
            {
                units.Add(context.Inventories.FirstOrDefault(i => i.ItemId == messageObject.ItemId));
            }

            var answer = JsonSerializer.Serialize(units);

            SendMessage(answer, "catalog");
        }
        catch (JsonException ex)
        {
            //TODO: send to invalid message queue
        }
    }
}
public class MessageObject
{
    public int ItemId { get; set; }
}
