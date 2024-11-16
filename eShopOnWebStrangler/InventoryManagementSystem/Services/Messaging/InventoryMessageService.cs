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

    public async Task SendMessage(string message, string _routingKey)
    {

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic, durable: true);


        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: exchangeName,
                             routingKey: _routingKey,
                             body: body);
        Console.WriteLine($" [x] Sent '{_routingKey}':'{message}'");
    }

    //TODO: add cancelation token
    public async Task ReceiveMessage(string routingKey, string queueName = "inventoryRequestQueue")
    {
        var factory = new ConnectionFactory { HostName = "localhost"/*, DispatchConsumersAsync = true*/ };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

        //Console.WriteLine("Waiting for messages");

        var consumer = new AsyncEventingBasicConsumer(channel);
        var latch = new AutoResetEvent(false);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var Message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"Received: {Message}");

            await ProcessMessage(Message);

            latch.Set();
        };
        //ConsumerReceived;

        await channel.BasicConsumeAsync(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        Console.ReadKey();
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

            await SendMessage(answer, "inventory_amount");
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
