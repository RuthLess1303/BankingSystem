namespace EmailSendingService;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _services;
    
    public Worker(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _services.CreateScope();
        var emailService = scope.ServiceProvider.GetRequiredService<ISendEmailService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            await emailService.SendEmail();
            await emailService.ChangeEmailStatusToBlocked();
            await Task.Delay(50 * 60, stoppingToken);
        }
    }
}