namespace eShopOnWebCatalog.Interfaces;

public interface IMessagingService
{
    public void SendMessage(string message,string rountingkey);
    public async Task ReceiveMessage(string routingKey, string queueName,CancellationToken cancellationToken) { }
}
