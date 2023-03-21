using AtmCore.Repositories;
using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtmTest;

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
        var transaction1 = new TransactionEntity
        {
            Id = 1,
            AggressorIban = "SENDERIBAN1",
            ReceiverIban = iban,
            Type = "ATM",
            Amount = 50,
            TransactionTime = DateTime.UtcNow.AddDays(-0.5),
            CurrencyCode = "Usd"
        };
        var transaction2 = new TransactionEntity
        {
            Id = 2,
            AggressorIban = "SENDERIBAN2",
            ReceiverIban = iban,
            Type = "ATM",
            Amount = 100,
            TransactionTime = DateTime.UtcNow.AddDays(-2),
            CurrencyCode = "Usd"
        };
        await _dbContext.Transaction.AddRangeAsync(transaction1, transaction2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _transactionRepository.GetWithdrawalsInLast24HoursAsync(iban);

        // Assert
        Assert.That(result, Is.EqualTo(50));
    }

    [Test]
    public async Task AddTransactionInDb_AddsTransactionToDatabase()
    {
        // Arrange
        var transaction = new TransactionEntity
        {
            Id = 1,
            AggressorIban = "SENDERIBAN",
            ReceiverIban = "RECEIVERIBAN",
            Type = "TRANSFER",
            Amount = 500,
            TransactionTime = DateTime.UtcNow,
            CurrencyCode = "Usd"
        };

        // Act
        await _transactionRepository.AddTransactionInDb(transaction);

        // Assert
        var result = await _dbContext.Transaction.FindAsync(transaction.Id);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.EqualTo(transaction));
    }
}