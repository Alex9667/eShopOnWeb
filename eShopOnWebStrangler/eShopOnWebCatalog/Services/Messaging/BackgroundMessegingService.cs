
using System.Threading;
using eShopOnWebCatalog.Interfaces;

namespace eShopOnWebCatalog.Services.Messaging;

public class BackgroundMessegingService : IHostedService
{
    IServiceProvider _serviceProvider;

    private Timer _timer;
    Task? ReciveMessage;
    public BackgroundMessegingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
        var scope = _serviceProvider.CreateAsyncScope();
        var messegingService = scope.ServiceProvider.GetRequiredService<IMessagingService>();
        messegingService.CancellationToken = cancellationToken;
        messegingService.ReceiveMessage("get_catalog", "catalogRequestQueue");
        Console.WriteLine("Messaging service started");
    }
   

    public Task StopAsync(CancellationToken cancellationToken)
    {

        //ReciveMessage.Dispose();
        return Task.CompletedTask;
    }
}
