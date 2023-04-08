using EmailSendingService;
using EmailSendingService.Repositories;
using InternetBank.Db.Db;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("AppDbContext")));
        services.AddTransient<ISendEmailService, SendEmailService>();
        services.AddTransient<IEmailSenderRepository, EmailSenderRepository>();
    })
    .Build();

host.Run();