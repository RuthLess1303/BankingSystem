using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Requests;
using BankingSystem.Atm.Core.Services;
using BankingSystem.Atm.Core.Validations;
using InternetBank.Atm.Core.Services;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test.ServicesTests;

[TestFixture]
public class BalanceServiceTests
{
    [SetUp]
    public void Setup()
    {
        // Set up the in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        // Set up the BalanceService with the in-memory database
        var accountRepository = new AccountRepository(_dbContext);
        var cardAuthService = new CardAuthService(accountRepository, new CardRepository(_dbContext),
            new WithdrawalRequestValidation());
        _balanceService = new BalanceService(accountRepository, cardAuthService);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    private AppDbContext _dbContext;
    private IBalanceService _balanceService;

    [Test]
    public async Task SeeBalance_WithValidCredentials_ReturnsAccountBalance()
    {
        // Arrange
        var account = new AccountEntity
        {
            Iban = "NL91ABNA0417164300",
            Balance = 1000.00m,
            CurrencyCode = "Usd",
            PrivateNumber = "1234567890"
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        var request = new AuthorizeCardRequest
        {
            CardNumber = "1111222233334444",
            PinCode = "1234"
        };

        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "1111222233334444",
            CardHolderName = "John Doe",
            Cvv = "123",
            Pin = "1234",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CreationDate = DateTime.UtcNow
        };
        var connection = new CardAccountConnectionEntity
        {
            Id = 1,
            CardId = card.Id,
            Iban = account.Iban,
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Card.Add(card);
        _dbContext.CardAccountConnection.Add(connection);
        await _dbContext.SaveChangesAsync();

        // Act
        var balance = await _balanceService.SeeBalance(request);

        // Assert
        Assert.That(balance, Is.EqualTo(account.Balance));
    }

    [Test]
    public void SeeBalance_WithInvalidCredentials_ThrowsArgumentException()
    {
        // Arrange
        var request = new AuthorizeCardRequest
        {
            CardNumber = "1111222233334444",
            PinCode = "9999"
        };

        // Act + Assert
        Assert.ThrowsAsync<ArgumentException>(() => _balanceService.SeeBalance(request));
    }
}