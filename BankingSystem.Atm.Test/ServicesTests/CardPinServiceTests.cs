using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Requests;
using BankingSystem.Atm.Core.Services;
using BankingSystem.Atm.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test.ServicesTests;

[TestFixture]
public class CardCardPinServiceTests
{
    private AppDbContext _dbContext;
    private ICardAuthService _cardAuthService;
    private ICardRepository _cardRepository;
    private IPinRepository _pinRepository;
    private ICardPinService _cardPinService;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;
        _dbContext = new AppDbContext(options);

        _cardRepository = new CardRepository(_dbContext);
        _cardAuthService = new CardAuthService(new AccountRepository(_dbContext), _cardRepository,
            new WithdrawalRequestValidation());
        _pinRepository = new CardPinRepository(_dbContext);
        _cardPinService = new CardCardPinService(_cardRepository, _cardAuthService, _pinRepository,new WithdrawalRequestValidation());
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task ChangeCardPin_ValidRequest_ChangesPin()
    {
        const string cardNumber = "341824880203048";
        const string pin = "1234";
        const string newPin = "4321";

        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "NL91ABNA0417164300",
            Balance = 1000.00m,
            CurrencyCode = "Usd",
            PrivateNumber = "1234567890",
            CreationDate = DateTime.UtcNow.AddYears(-6)
        };
        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = cardNumber,
            CardHolderName = "John Doe",
            Cvv = "123",
            Pin = pin,
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CreationDate = DateTime.UtcNow.AddYears(-6)
        };
        var connection = new CardAccountConnectionEntity
        {
            Id = 1,
            CardId = card.Id,
            Iban = account.Iban,
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Card.Add(card);
        _dbContext.Account.Add(account);
        _dbContext.CardAccountConnection.Add(connection);
        await _dbContext.SaveChangesAsync();

        var request = new ChangePinRequest { CardNumber = cardNumber, PinCode = pin, NewPin = newPin };

        await _cardPinService.ChangeCardPin(request);

        var updatedCard = await _cardRepository.FindCardEntityByCardNumberAsync(cardNumber);
        Assert.That(updatedCard.Pin, Is.EqualTo(newPin));
    }

    [Test]
    public async Task ChangeCardPin_InvalidRequest_ThrowsException()
    {
        const string cardNumber = "1234567890123456";
        const string pin = "1234";
        const string newPin = "4321";

        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "1234567890123456",
            CardHolderName = "John Doe",
            Cvv = "123",
            Pin = "1234",
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CreationDate = DateTime.UtcNow
        };
        _dbContext.Card.Add(card);
        await _dbContext.SaveChangesAsync();

        var request = new ChangePinRequest { CardNumber = cardNumber, PinCode = "0000", NewPin = newPin };

        Assert.ThrowsAsync<ArgumentException>(() => _cardPinService.ChangeCardPin(request));

        var updatedCard = await _cardRepository.FindCardEntityByCardNumberAsync(cardNumber);
        Assert.That(updatedCard.Pin, Is.EqualTo(pin));
    }
}