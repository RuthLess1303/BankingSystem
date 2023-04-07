using BankingSystem.Atm.Api.Middlewares;
using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Services;
using BankingSystem.Atm.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Repositories;
using Microsoft.EntityFrameworkCore;
using AccountRepository = BankingSystem.Atm.Core.Repositories.AccountRepository;
using CardRepository = BankingSystem.Atm.Core.Repositories.CardRepository;
using IAccountRepository = BankingSystem.Atm.Core.Repositories.IAccountRepository;
using ICardRepository = BankingSystem.Atm.Core.Repositories.ICardRepository;
using ITransactionRepository = BankingSystem.Atm.Core.Repositories.ITransactionRepository;
using TransactionRepository = BankingSystem.Atm.Core.Repositories.TransactionRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(c =>
    c.UseSqlServer(builder.Configuration["AppDbContext"]));

builder.Services.AddTransient<IBalanceService, BalanceService>();
builder.Services.AddTransient<ICardAuthService, CardAuthService>();
builder.Services.AddTransient<ICardPinService, CardCardPinService>();
builder.Services.AddTransient<IWithdrawalService, WithdrawalService>();
builder.Services.AddTransient<IAccountRepository, AccountRepository>();
builder.Services.AddTransient<IPinRepository, CardPinRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
builder.Services.AddTransient<ICardRepository, CardRepository>();
builder.Services.AddTransient<IWithdrawalRequestValidation, WithdrawalRequestValidation>();
builder.Services.AddTransient<ILoggerRepository, LoggerRepository>();

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