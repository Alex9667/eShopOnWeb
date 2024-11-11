
using eShopOnWebCatalog.Interfaces;

namespace eShopOnWebCatalog.Services.Messaging;

public class BackgroundMessegingService : IHostedService
{
    IServiceProvider _serviceProvider;
    Task? ReciveMessage;
    public BackgroundMessegingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var messegingService = scope.ServiceProvider.GetRequiredService<IMessagingService>();
            ReciveMessage = messegingService.ReceiveMessage("get_catalog", "catalogRequestQueue",cancellationToken);
            
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        cancellationToken.WaitHandle.WaitOne();
        ReciveMessage.Dispose();
        return Task.CompletedTask;
    }
}
