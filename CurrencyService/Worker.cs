namespace CurrencyService;

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
        var currencyService = scope.ServiceProvider.GetRequiredService<ICurrencyService>();

        await currencyService.AddInDb();

        while (!stoppingToken.IsCancellationRequested)
        {
            // AddInDb method is called every day at midnight
            var nextRunTime = DateTime.Today.AddDays(1);
            var delay = nextRunTime - DateTime.Now;

            await Task.Delay(delay, stoppingToken);
            await currencyService.AddInDb();
        }
    }
}