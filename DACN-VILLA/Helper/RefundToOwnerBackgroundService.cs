using DACN_VILLA.Interface.Service;

public class RefundToOwnerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RefundToOwnerBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

            // Use bookingService to perform tasks
            await bookingService.RefundToOwnerAsync();
        }
    }
}
