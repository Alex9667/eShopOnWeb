namespace eShopOnWebCatalog.Services.Messaging;

using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using eShopOnWebCatalog.Entities;
using eShopOnWebCatalog.Infrastructure;
using eShopOnWebCatalog.Specifications;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using eShopOnWebCatalog.Interfaces;
using System.Threading;

public class CatalogMessageService : IMessagingService /*IHostedService*/ 
{
    ConnectionFactory factory;
    IRepository<CatalogItem> _itemRepository;

    string exchangeName = "";

    public CatalogMessageService(IRepository<CatalogItem> itemRepository)
    {
        factory = new ConnectionFactory
        {
            HostName = "localhost"
        };
        exchangeName = "ewebshop";
        _itemRepository = itemRepository;

    }
    //private static CatalogMessageService _messagingService = null;

    //public static CatalogMessageService messagingService
    //{
    //    get
    //    {
    //        if (_messagingService == null)
    //        {
    //            _messagingService = new CatalogMessageService();
    //        }
    //        return _messagingService;
    //    }
    //}

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
    public async Task ReceiveMessage(string routingKey, string queueName = "catalogRequestQueue")
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

        consumer.Received += ConsumerReceived;

        channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
    }

    public async Task ConsumerReceived(object sender, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        MessageObject[] messageObjects;
        try
        {
            messageObjects = JsonSerializer.Deserialize<MessageObject[]>(message);
            var catalogItemsSpecification = new CatalogItemsSpecification(messageObjects.Select(M => M.ID).ToArray());
            var catalogitems = await _itemRepository.ListAsync(catalogItemsSpecification);
            var answer = JsonSerializer.Serialize(catalogitems);
            SendMessage(answer, "catalog");
        }
        catch (JsonException ex)
        {
            //TODO: send to invalid message queue
        }

        Console.WriteLine($"Received: {message}");
    }

    //public async Task StartAsync(CancellationToken cancellationToken)
    //{
    //    _token = cancellationToken;
    //    ListenForMesages =  ReceiveMessage("get_catalog");
    //    await ListenForMesages;
    //}

    //public Task StopAsync(CancellationToken cancellationToken)
    //{
    //    ListenForMesages.Dispose();
    //    return Task.CompletedTask;
    //}
}
public class MessageObject
{
    public int ID { get; set; }
}
