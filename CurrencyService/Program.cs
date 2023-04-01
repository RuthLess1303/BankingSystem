using InternetBank.Db.Db;
using InternetBank.Db.Db.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CurrencyService;

internal class Program
{
    private static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                services.AddScoped<ICurrencyRepository, CurrencyRepository>();
                services.AddScoped<ICurrencyService, CurrencyService>();
                services.AddHostedService<Worker>();
            });
    }
}