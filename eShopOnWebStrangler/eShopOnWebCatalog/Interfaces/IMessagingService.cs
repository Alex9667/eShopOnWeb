namespace eShopOnWebCatalog.Interfaces;

public interface IMessagingService
{
    CancellationToken CancellationToken { get; set; }
    public async Task SendMessage(string message, string rountingkey) { }
    public async Task ReceiveMessage(string routingKey, string queueName) { }
}
