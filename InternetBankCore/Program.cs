using InternetBankCore.Db;
using InternetBankCore.Db.Repositories;
using InternetBankCore.Services;
using InternetBankCore.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddTransient<ICurrencyRepository, CurrencyRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IPropertyValidations, PropertyValidations>();
        services.AddTransient<IAccountValidation, AccountValidation>();
        services.AddTransient<ICardValidation, CardValidation>();
        services.AddTransient<ICurrencyService, CurrencyService>();
        services.AddTransient<IAccountRepository, AccountRepository>();
        services.AddTransient<ITransactionRepository, TransactionRepository>();
        services.AddTransient<ITransactionService, TransactionService>();
        services.AddTransient<IUserService, UserService>();
    })
    .Build();

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlServer("Server=localhost;Database=MobileBankDb;User Id=sa; Password=wavedi123;Encrypt=False;");