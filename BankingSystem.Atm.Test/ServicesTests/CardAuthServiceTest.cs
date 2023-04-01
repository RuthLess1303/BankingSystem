using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Services;
using BankingSystem.Atm.Core.Validations;
using InternetBank.Atm.Core.Services;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test.ServicesTests;

[TestFixture]
public class CardAuthServiceTests
{
    private AppDbContext _dbContext;
    private IAccountRepository _accountRepository;
    private ICardRepository _cardRepository;
    private IWithdrawalRequestValidation _requestValidation;
    private ICardAuthService _cardAuthService;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _accountRepository = new AccountRepository(_dbContext);
        _cardRepository = new CardRepository(_dbContext);
        _requestValidation = new WithdrawalRequestValidation();

        _cardAuthService = new CardAuthService(
            _accountRepository,
            _cardRepository,
            _requestValidation);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task GetAuthorizedAccountAsync_ShouldReturnCorrectAccount()
    {
        // Arrange
        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = "12345678901",
            Iban = "GB29NWBK60161331926819",
            CurrencyCode = "GBP",
            Balance = 1000,
            CreationDate = DateTime.UtcNow
        };
        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "4111111111111111",
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
        _dbContext.Account.Add(account);
        _dbContext.Card.Add(card);
        _dbContext.CardAccountConnection.Add(connection);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cardAuthService.GetAuthorizedAccountAsync(card.CardNumber, card.Pin);

        // Assert
        Assert.That(result, Is.EqualTo(account));
    }

    [Test]
    public async Task GetAuthorizedAccountAsync_ShouldThrowException_WhenCardIsExpired()
    {
        // Arrange
        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "4111111111111111",
            CardHolderName = "John Doe",
            Cvv = "123",
            Pin = "1234",
            ExpirationDate = DateTime.UtcNow.AddDays(-1),
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Card.Add(card);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        var ex = Assert.ThrowsAsync<UnauthorizedAccessException>(async () =>
        {
            await _cardAuthService.GetAuthorizedAccountAsync(card.CardNumber, card.Pin);
        });
        Assert.That(ex.Message, Is.EqualTo("Card has expired"));
    }
}