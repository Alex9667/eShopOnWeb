
using System.Threading;
using eShopOnWebCatalog.Interfaces;

namespace eShopOnWebCatalog.Services.Messaging;

public class BackgroundMessegingService : IHostedService
{
    IServiceProvider _serviceProvider;
    private Timer _timer;
    //Task? ReciveMessage;
    public BackgroundMessegingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ReadMesseges, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }
    public async void ReadMesseges(object? state)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var messegingService = scope.ServiceProvider.GetRequiredService<IMessagingService>();
            await messegingService.ReceiveMessage("get_catalog", "catalogRequestQueue");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer.Dispose();
        //ReciveMessage.Dispose();
        return Task.CompletedTask;
    }
}
