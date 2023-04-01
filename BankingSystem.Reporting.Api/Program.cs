using InternetBank.Core.Services;
using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Repositories;
using BankingSystem.Reporting.Api.Middlewares;
using BankingSystem.Reporting.Api.Services;
using BankingSystem.Reporting.Core.Repositories;
using BankingSystem.Reporting.Core.Validations;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(c =>
    c.UseSqlServer(builder.Configuration["AppDbContext"]));

// Add services to the container.
builder.Services.AddTransient<ITransactionStatisticsRepository, TransactionStatisticsRepository>();
builder.Services.AddTransient<IUserStatisticsRepository, UserStatisticsRepository>();
builder.Services.AddTransient<ITransactionStatisticsValidations, TransactionStatisticsValidations>();
builder.Services.AddTransient<ITransactionStatisticsService, TransactionStatisticsService>();
builder.Services.AddTransient<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddTransient<ITransactionValidations, TransactionValidations>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();