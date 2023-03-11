using InternetBankCore.Services;
using InternetBankCore.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<IPropertyValidations, PropertyValidations>();
        services.AddTransient<IAccountValidation, AccountValidation>();
        services.AddTransient<ICardValidation, CardValidation>();
        services.AddTransient<ICurrencyService, CurrencyService>();
        services.AddTransient<ITransactionService, TransactionService>();
        services.AddTransient<IUserService, UserService>();
    })
    .Build();
