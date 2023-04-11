using BankingSystem.Atm.Core.Repositories;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test;

[TestFixture]
public class TransactionRepositoryTests
{
    private AppDbContext _dbContext;
    private ITransactionRepository _transactionRepository;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _transactionRepository = new TransactionRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task GetWithdrawalsInLast24HoursAsync_ReturnsCorrectValue()
    {
        // Arrange
        const string iban = "TESTIBAN";
        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = iban,
            Balance = 1000,
            CurrencyCode = "Usd",
            CreationDate = DateTime.UtcNow.AddDays(-80),
            PrivateNumber = "1234567890",
        };
        
        var user = new UserEntity
        {
            FirstName = "John",
            LastName = "Doe",
            PrivateNumber = "1234567890",
            BirthDate = new DateTime(1990, 1, 1),
            CreationDate = DateTime.UtcNow.AddDays(-900),
            Email = "John@Mail.com"
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.Account.AddAsync(account);
        await _dbContext.SaveChangesAsync();
        
        var transaction1 = new TransactionEntity
        {
            Id = 1,
            SenderIban = "SENDERIBAN1",
            ReceiverIban = iban,
            Type = "ATM",
            Amount = 50,
            TransactionTime = DateTime.UtcNow.AddDays(-0.5),
            CurrencyCode = "Usd"
        };
        var transaction2 = new TransactionEntity
        {
            Id = 2,
            SenderIban = "SENDERIBAN2",
            ReceiverIban = iban,
            Type = "ATM",
            Amount = 100,
            TransactionTime = DateTime.UtcNow.AddDays(-0.7),
            CurrencyCode = "Usd"
        };
        await _dbContext.Transaction.AddRangeAsync(transaction1, transaction2);
        await _dbContext.SaveChangesAsync();

        var result = await _transactionRepository.GetWithdrawalAmountInLast24HoursAsync(iban);

        Assert.That(result, Is.EqualTo(150));
    }

    [Test]
    public async Task AddTransactionInDb_AddsTransactionToDatabase()
    {
        var transaction = new TransactionEntity
        {
            Id = 1,
            SenderIban = "SENDERIBAN",
            ReceiverIban = "RECEIVERIBAN",
            Type = "TRANSFER",
            Amount = 500,
            TransactionTime = DateTime.UtcNow,
            CurrencyCode = "Usd"
        };
        
        await _transactionRepository.AddTransactionInDb(transaction);

        var result = await _dbContext.Transaction.FindAsync(transaction.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(transaction));
    }
}