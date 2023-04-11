using BankingSystem.Atm.Core.Repositories;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test;

[TestFixture]
public class AccountRepositoryTests
{
    private AppDbContext _dbContext;
    private IAccountRepository _accountRepository;
    
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();

        _accountRepository = new AccountRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task GetAccountByCardDetails_WhenValidCardDetailsProvided_ReturnsAccount()
    {
        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "1234567890123456",
            Pin = "1234",
            CreationDate = DateTime.Now,
            Cvv = "561",
            CardHolderName = "Nick"
        };
        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "NL91ABNA0417164300",
            Balance = 5000,
            CreationDate = DateTime.Now,
            CurrencyCode = "Usd",
            PrivateNumber = "1234567890"
        };
        var cardAccountConnection = new CardAccountConnectionEntity
        {
            Id = 1,
            CardId = card.Id,
            Iban = account.Iban,
            CreationDate = DateTime.Now
        };
        _dbContext.Card.Add(card);
        _dbContext.Account.Add(account);
        _dbContext.CardAccountConnection.Add(cardAccountConnection);
        await _dbContext.SaveChangesAsync();

        var result = await _accountRepository.GetAccountByCardDetails(card.CardNumber, card.Pin);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Iban, Is.EqualTo(account.Iban));
            Assert.That(result.Balance, Is.EqualTo(account.Balance));
        });
    }

    [Test]
    public void GetAccountByCardDetails_WhenInvalidCardDetailsProvided_ThrowsException()
    {
        const string invalidCardNumber = "1234567890123456";
        const string invalidPin = "0000";

        Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _accountRepository.GetAccountByCardDetails(invalidCardNumber, invalidPin));
    }

    [Test]
    public async Task GetAccountMoney_WhenValidIbanProvided_ReturnsBalance()
    {
        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "NL91ABNA0417164300",
            Balance = 5000,
            CreationDate = DateTime.Now,
            CurrencyCode = "Usd",
            PrivateNumber = "1234567890"
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        var result = await _accountRepository.GetAccountBalance(account.Iban);

        Assert.That(result, Is.EqualTo(account.Balance));
    }

    [Test]
    public void GetAccountMoney_WhenInvalidIbanProvided_ThrowsException()
    {
        const string invalidIban = "invalid_iban";

        Assert.ThrowsAsync<ArgumentException>(() => _accountRepository.GetAccountBalance(invalidIban));
    }
}