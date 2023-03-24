using InternetBank.Atm.Core.Repositories;
using InternetBank.Atm.Core.Requests;
using InternetBank.Atm.Core.Services;
using InternetBank.Atm.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Atm.Test.ServicesTests;

[TestFixture]
public class CardAuthServiceTests
{
    private AppDbContext _context;
    private IAccountRepository _accountRepository;
    private ICardRepository _cardRepository;
    private WithdrawalRequestValidation _requestValidation;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        _accountRepository = new AccountRepository(_context);
        _cardRepository = new CardRepository(_context);
        _requestValidation = new WithdrawalRequestValidation();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetAuthorizedAccountAsync_InvalidPin_ThrowsArgumentException()
    {
        // Arrange
        _context.Account.Add(new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = "1234567890",
            Iban = "GB12TEST3456789012",
            CurrencyCode = "GBP",
            Balance = 1000,
            CreationDate = DateTime.UtcNow
        });

        _context.CardAccountConnection.Add(new CardAccountConnectionEntity
        {
            Id = 1,
            CardId = Guid.NewGuid(),
            Iban = "GB12TEST3456789012",
            CreationDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        var sut = new CardAuthService(
            new AccountRepository(_context),
            new CardRepository(_context),
            new WithdrawalRequestValidation() // Use WithdrawalRequestValidator instead of WithdrawalRequestValidation
        );

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => sut.GetAuthorizedAccountAsync(cardNumber :"1234567890123456", pin:"1234"));
    }


    [Test]
    public Task GetAuthorizedAccountAsync_InvalidCardNumber_ThrowsArgumentException()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            CardNumber = "123456789012345", // 15 digits instead of 16
            PinCode = "1234",
            Amount = 100
        };
        _requestValidation.ValidateCreditCardNumber(request.CardNumber);

        var sut = new CardAuthService(_accountRepository, _cardRepository, _requestValidation);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetAuthorizedAccountAsync(request.CardNumber,request.PinCode));
        return Task.CompletedTask;
    }

    [Test]
    public Task GetAuthorizedAccountAsync_CardDoesNotExist_ThrowsArgumentException()
    {
        // Arrange
        var request = new WithdrawalRequest
        {
            CardNumber = "1234567890123456",
            PinCode = "1234",
            Amount = 100
        };
        _requestValidation.ValidatePinCode(request.PinCode);
        _requestValidation.ValidateCreditCardNumber(request.CardNumber);

        var sut = new CardAuthService(_accountRepository, _cardRepository, _requestValidation);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await sut.GetAuthorizedAccountAsync(request.CardNumber,request.PinCode));
        return Task.CompletedTask;
    }
}