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
public class WithdrawalServiceTests
{
    private AppDbContext _dbContext;
    private ICardAuthService _cardAuthService;
    private ICardRepository _cardRepository;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _cardRepository = new CardRepository(_dbContext);
        _cardAuthService = new CardAuthService(new AccountRepository(_dbContext), _cardRepository,
            new WithdrawalRequestValidation());
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task Withdraw_ValidRequest_DecrementsAccountBalance()
    {
        // Arrange
        var user = new UserEntity
        {
            UserName = "john.doe@example.com",
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            PrivateNumber = "12345678901",
            BirthDate = new DateTime(1990, 1, 1),
            CreationDate = DateTime.UtcNow
        };
        var account1 = new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = "12345678901",
            Iban = "GB29NWBK60161331926819",
            CurrencyCode = "Gel",
            Balance = 1000,
            CreationDate = DateTime.UtcNow
        };
        var account2 = new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = "12345678901",
            Iban = "GB29NWBK60161331925432",
            CurrencyCode = "Gel",
            Balance = 10000,
            CreationDate = DateTime.UtcNow
        };
        
        var card1 = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "1234123412341234",
            CardHolderName = "John Doe",
            Cvv = "123",
            Pin = "1234",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CreationDate = DateTime.UtcNow
        };
        var card2 = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "13421433244324242",
            CardHolderName = "John Doe",
            Cvv = "332",
            Pin = "1234",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CreationDate = DateTime.UtcNow
        };
        var connection1 = new CardAccountConnectionEntity
        {
            Id = 1,
            CardId = card1.Id,
            Iban = account1.Iban,
            CreationDate = DateTime.UtcNow
        };
        var connection2 = new CardAccountConnectionEntity
        {
            Id = 2,
            CardId = card2.Id,
            Iban = account2.Iban,
            CreationDate = DateTime.UtcNow
        };
        await _dbContext.User.AddAsync(user);
        await _dbContext.CardAccountConnection.AddAsync(connection1);
        await _dbContext.CardAccountConnection.AddAsync(connection2);
        await _dbContext.Account.AddAsync(account1);
        await _dbContext.Account.AddAsync(account2);
        await _dbContext.Card.AddAsync(card1);
        await _dbContext.Card.AddAsync(card2);
        await _dbContext.SaveChangesAsync();

        var withdrawalService = new WithdrawalService(_cardAuthService, new TransactionRepository(_dbContext));

        var request1 = new WithdrawalRequest
        {
            CardNumber = "1234123412341234",
            PinCode = "1234",
            Amount = 500
        };
        
        var request2 = new WithdrawalRequest
        {
            CardNumber = "13421433244324242",
            PinCode = "1234",
            Amount = 2000
        };

        // Act
        await withdrawalService.Withdraw(request1);
        await withdrawalService.Withdraw(request2);

        // Assert
        var updatedAccount1 = await _dbContext.Account.FirstOrDefaultAsync(a => a.Iban == account1.Iban);
        var updatedAccount2 = await _dbContext.Account.FirstOrDefaultAsync(a => a.Iban == account2.Iban);
        Assert.Multiple(() =>
        {
            Assert.That(updatedAccount1.Balance, Is.EqualTo(490));
            Assert.That(updatedAccount2.Balance, Is.EqualTo(7960));
        });
    }

    [Test]
    public Task Withdraw_InvalidRequest_ThrowsException()
    {
        // Arrange

        var withdrawalService = new WithdrawalService(_cardAuthService, new TransactionRepository(_dbContext));

        var request = new WithdrawalRequest
        {
            CardNumber = "1234123412341234",
            PinCode = "1234",
            Amount = 1000
        };

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => withdrawalService.Withdraw(request));
        return Task.CompletedTask;
    }
}