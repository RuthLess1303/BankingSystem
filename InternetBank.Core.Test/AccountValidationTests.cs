using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Core.Test;

[TestFixture]
public class AccountValidationTests
{
    private IAccountValidation _accountValidation;
    private IPropertyValidations _propertyValidations;
    private IAccountRepository _accountRepository;
    private DbContextOptions<AppDbContext> _options;
    private AppDbContext _dbContext;
    
    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("InternetBankDb")
            .Options;

        _dbContext = new AppDbContext(_options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();


        _propertyValidations = new PropertyValidations(
            new CurrencyRepository(_dbContext),
            new UserRepository(_dbContext, new UserManager<UserEntity>(
                new UserStore<UserEntity, IdentityRole<Guid>, AppDbContext, Guid>(_dbContext),
                null, null, null, null, null, null, null, null)),
            new AccountRepository(_dbContext),
            new CardRepository(_dbContext)
        );
        _accountRepository = new AccountRepository(_dbContext);
        _accountValidation = new AccountValidation(_propertyValidations, _accountRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public Task OnCreate_ThrowsException_WhenAmountIsNegative()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            Amount = -100,
            Iban = "EE0011112222",
            CurrencyCode = "EUR",
            PrivateNumber = "12345678901"
        };

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _accountValidation.OnCreate(request));
        return Task.CompletedTask;
    }

    [Test]
    public Task OnCreate_ThrowsException_WhenIbanIsInvalid()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            Amount = 1000,
            Iban = "invalid_iban",
            CurrencyCode = "EUR",
            PrivateNumber = "12345678901"
        };

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _accountValidation.OnCreate(request));
        return Task.CompletedTask;
    }

    [Test]
    public Task OnCreate_ThrowsException_WhenCurrencyIsInvalid()
    {
        // Arrange
        var request = new CreateAccountRequest
        {
            Amount = 1000,
            Iban = "EE0011112222",
            CurrencyCode = "INVALID_CURRENCY",
            PrivateNumber = "12345678901"
        };

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _accountValidation.OnCreate(request));
        return Task.CompletedTask;
    }

    [Test]
    public async Task OnCreate_ThrowsException_WhenPrivateNumberIsAlreadyUsed()
    {
        // Arrange
        var dbContext = new AppDbContext(_options);
        dbContext.Account.Add(new AccountEntity
        {
            Iban = "EE0011112222",
            PrivateNumber = "12345678901",
            CurrencyCode = "Usd"
        });
        await dbContext.SaveChangesAsync();

        var request = new CreateAccountRequest
        {
            Amount = 1000,
            Iban = "EE0022223333",
            CurrencyCode = "EUR",
            PrivateNumber = "12345678901"
        };

        _accountValidation = new AccountValidation(_propertyValidations, _accountRepository);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(() => _accountValidation.OnCreate(request));
    }

    [Test]
    public async Task AccountWithIbanExists_Returns_True_When_Account_Exists()
    {
        // Arrange
        var account = new AccountEntity
        {
            Iban = "EE0011112222",
            PrivateNumber = "12345678901",
            CurrencyCode = "Usd"
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _accountValidation.AccountWithIbanExists("EE0011112222");

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsCurrencySame_Returns_True_When_Currencies_Are_Same()
    {
        // Arrange
        const string aggressorIban = "EE0011112222";
        const string receiverIban = "EE0033334444";
        var aggressorAccount = new AccountEntity
        {
            Iban = aggressorIban,
            PrivateNumber = "12345678901",
            CurrencyCode = "USD"
        };
        var receiverAccount = new AccountEntity
        {
            Iban = receiverIban,
            PrivateNumber = "23456789012",
            CurrencyCode = "USD"
        };
        _dbContext.Account.AddRange(aggressorAccount, receiverAccount);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _accountValidation.IsCurrencySame(aggressorIban, receiverIban);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task IsCurrencySame_Returns_False_When_Currencies_Are_Different()
    {
        // Arrange
        const string aggressorIban = "EE0011112222";
        const string receiverIban = "EE0033334444";
        var aggressorAccount = new AccountEntity
        {
            Iban = aggressorIban,
            PrivateNumber = "12345678901",
            CurrencyCode = "USD"
        };
        var receiverAccount = new AccountEntity
        {
            Iban = receiverIban,
            PrivateNumber = "23456789012",
            CurrencyCode = "EUR"
        };
        _dbContext.Account.AddRange(aggressorAccount, receiverAccount);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _accountValidation.IsCurrencySame(aggressorIban, receiverIban);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasSufficientBalance_Throws_Exception_When_Balance_Is_Less_Than_Amount()
    {
        // Arrange
        var account = new AccountEntity
        {
            Iban = "EE0011112222",
            PrivateNumber = "12345678901",
            CurrencyCode = "USD",
            Balance = 100m
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        // Act and Assert
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await _accountValidation.HasSufficientBalance("EE0011112222", 200m));
        Assert.That(ex.Message, Is.EqualTo("Insufficient balance"));
    }

    [Test]
    public async Task GetAmountWithIban_Returns_Account_Balance()
    {
        // Arrange
        var account = new AccountEntity
        {
            Iban = "EE0011112222",
            PrivateNumber = "12345678901",
            CurrencyCode = "Usd",
            Balance = 100.0m
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _accountValidation.GetAmountWithIban("EE0011112222");

        // Assert
        Assert.That(result, Is.EqualTo(100.0m));
    }

    [Test]
    public async Task GetAccountWithIban_Returns_Account_When_Exists()
    {
        // Arrange
        var account = new AccountEntity
        {
            Iban = "EE0011112222",
            PrivateNumber = "12345678901",
            CurrencyCode = "Usd",
            Balance = 100
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _accountRepository.GetAccountWithIban("EE0011112222");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<AccountEntity>());
    }

    [Test]
    public async Task HasTransaction_Returns_True_When_Transaction_Exists()
    {
        // Arrange
        var transaction = new TransactionEntity
        {
            Id = 1,
            Amount = 100,
            GrossAmount = 105,
            SenderIban = "EE0011112222",
            ReceiverIban = "GB29NWBK60161331926821",
            CurrencyCode = "USD",
            Fee = 5,
            Type = "Transfer",
            Rate = 1.05m,
            TransactionTime = DateTime.UtcNow
        };

        _dbContext.Transaction.Add(transaction);
        await _dbContext.SaveChangesAsync();


        // Act
        var result = await _accountRepository.HasTransaction("GB29NWBK60161331926821");

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetTransactionsWithIban_Returns_All_Transactions_For_Given_Iban()
    {
        // Arrange

        const string iban = "GB29NWBK60161331926821";
        var transaction1 = new TransactionEntity
        {
            SenderIban = iban, ReceiverIban = "TR987654321098765432109876", Amount = 100,
            TransactionTime = DateTime.UtcNow, CurrencyCode = "Usd", Type = "Transfer"
        };
        var transaction2 = new TransactionEntity
        {
            SenderIban = "EE0011112222", ReceiverIban = iban, Amount = 200, TransactionTime = DateTime.UtcNow,
            CurrencyCode = "Gel", Type = "Transfer"
        };
        var transaction3 = new TransactionEntity
        {
            SenderIban = iban, ReceiverIban = "US123456789012345678901234", Amount = 300,
            TransactionTime = DateTime.UtcNow, CurrencyCode = "Eur", Type = "Transfer"
        };

        _dbContext.Transaction.AddRange(transaction1, transaction2, transaction3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _accountValidation.GetTransactionsWithIban(iban);

        // Assert
        Assert.That(result, Has.Count.EqualTo(3));
        CollectionAssert.Contains(result, transaction1);
        CollectionAssert.Contains(result, transaction2);
        CollectionAssert.Contains(result, transaction3);
    }

    [Test]
    public async Task GetCardWithIban_Returns_Card_When_Exists()
    {
        // Arrange
        const string iban = "GB29NWBK60161331926821";
        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = "1111222233334444",
            CardHolderName = "John Doe",
            Cvv = "123",
            Pin = "1234",
            ExpirationDate = new DateTime(2025, 12, 31),
            CreationDate = DateTime.UtcNow
        };
        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = iban,
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

        // Act
        var result = await _accountRepository.GetCardWithIban(iban);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Id, Is.EqualTo(card.Id));
            Assert.That(result.CardNumber, Is.EqualTo(card.CardNumber));
            Assert.That(result.ExpirationDate, Is.EqualTo(card.ExpirationDate));
            Assert.That(result.Cvv, Is.EqualTo(card.Cvv));
            Assert.That(result.CardHolderName, Is.EqualTo(card.CardHolderName));
        });
    }
}