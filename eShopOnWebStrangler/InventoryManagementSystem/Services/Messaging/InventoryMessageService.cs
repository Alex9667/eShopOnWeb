using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using RabbitMQ.Client.Exceptions;

namespace InventoryManagementSystem.Services.Messaging;

internal class InventoryMessageService
{
    ConnectionFactory factory;

    private InventoryDbContext context;
    private readonly RabbitMqSettings _settings;

    string exchangeName = "";

    public InventoryMessageService(InventoryDbContext context, RabbitMqSettings settings)
    {
        _settings = settings;
        factory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password
        };
        exchangeName = "ewebshop";

        this.context = context;

    }

    public async Task SendMessage(string message, string _routingKey, string sendQueueName)
    {

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Topic, durable: true);
        await channel.QueueDeclareAsync(queue: sendQueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        await channel.QueueBindAsync(queue: sendQueueName, exchange: exchangeName, routingKey: _routingKey);


        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: exchangeName,
                             routingKey: _routingKey,
                             body: body);
        Console.WriteLine($" [x] Sent '{_routingKey}':'{message}'");
    }

    //TODO: add cancelation token
    public async Task ReceiveMessage(string routingKey, string sendQueueName, string queueName = "inventoryRequestQueue")
    {
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

            await ProcessMessage(Message, sendQueueName);
        };
        //ConsumerReceived;

        await channel.BasicConsumeAsync(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
        Console.WriteLine("ready to recive");

        latch.WaitOne();
    }

    public async Task UpdateInventoryReceiver(string routingKey, string queueName = "inventoryUpdateQueue")
    {
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

            try
            {
                List<InventoryModel> itemsToReduce = new(JsonSerializer.Deserialize<InventoryModel[]>(Message));

                InventoryRepo inventoryRepo = new();
                inventoryRepo.ReduceInventoryAmount(itemsToReduce);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Failed to deserialize message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

        };
        //ConsumerReceived;

        await channel.BasicConsumeAsync(queue: queueName,
                             autoAck: true,
                             consumer: consumer);

        latch.WaitOne();
    }

    private async Task ProcessMessage(string message, string sendQueueName)
    {
        MessageObject[] messageObjects;
        List<InventoryModel> units = new();
        try
        {
            messageObjects = JsonSerializer.Deserialize<MessageObject[]>(message);

            foreach (var messageObject in messageObjects)
            {
                var unit = context.Inventories.FirstOrDefault(i => i.CatalogItemId == messageObject.Id);
                units.Add(unit);
                Console.WriteLine(unit.Units);
            }

            var answer = JsonSerializer.Serialize(units);

            await SendMessage(answer, "inventory_amount", sendQueueName);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            //TODO: send to invalid message queue
        }
    }
}
public class MessageObject
{
    public int Id { get; set; }
}
