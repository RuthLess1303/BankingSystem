using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Repositories;
using InternetBankCore.Services;
using InternetBankCore.Validations;
using Microsoft.EntityFrameworkCore;
using ReportingApi.Middlewares;
using ReportingApi.Services;
using ReportingCore.Validations;

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