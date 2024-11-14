using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using eShopOnWebCatalog.Interfaces;
using eShopOnWebCatalog.Entities;
using eShopOnWebCatalog.Specifications;

namespace eShopOnWebCatalog.Services.Messaging;

public class CatalogMessageService : BackgroundService
{
    //private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _exchangeName = "ewebshop";
    private IConnection _connection;
    private IModel _channel;

    public CatalogMessageService(/*IRepository<CatalogItem> itemRepository*/IServiceProvider serviceProvider)
    {
        //_itemRepository = itemRepository;
        _serviceProvider = serviceProvider;
        InitializeRabbitMQ();
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "localhost", DispatchConsumersAsync = true };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        string queueName = "catalogRequestQueue";
        string routingKey = "get_catalog";

        _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        _channel.QueueBind(queue: queueName, exchange: _exchangeName, routingKey: routingKey);
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += ConsumerReceived;

        _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

        Console.WriteLine("Waiting for messages...");

        await Task.CompletedTask;
    }

    private async Task ConsumerReceived(object sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"Received: {message}");

        using (var scope = _serviceProvider.CreateScope())
        {
            var itemRepository = scope.ServiceProvider.GetRequiredService<IRepository<CatalogItem>>();
            try
            {
                var messageObjects = JsonSerializer.Deserialize<MessageObject[]>(message);
                var catalogItemsSpecification = new CatalogItemsSpecification(messageObjects.Select(m => m.Id).ToArray());
                var catalogItems = await itemRepository.ListAsync(catalogItemsSpecification);
                var responseMessage = JsonSerializer.Serialize(catalogItems);

                SendMessage(responseMessage, "catalog");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                // Send to an invalid message queue or log appropriately
            }
        }
    }

    public void SendMessage(string message, string routingKey)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: _exchangeName, routingKey: routingKey, basicProperties: null, body: body);
        Console.WriteLine($"[x] Sent '{routingKey}':'{message}'");
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}

public class MessageObject
{
    public int Id { get; set; }
}
