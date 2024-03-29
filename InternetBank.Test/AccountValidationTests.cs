﻿using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Test;

[TestFixture]
public class AccountValidationTests
{
    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("InternetBankDb")
            .Options;

        _dbContext = new AppDbContext(_options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();


        var userStore = new UserStore<UserEntity, IdentityRole<int>, AppDbContext, int>(_dbContext);
        var passwordHasher = new PasswordHasher<UserEntity>();
        var userValidators = new List<IUserValidator<UserEntity>>();
        var passwordValidators = new List<IPasswordValidator<UserEntity>>();
        var keyNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        var userRepository = new UserRepository(_dbContext, new UserManager<UserEntity>(
            userStore,
            null, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, null, null));

        _propertyValidations = new PropertyValidations(
            new CurrencyRepository(_dbContext),
            userRepository,
            new AccountRepository(_dbContext)
        );
        _accountRepository = new AccountRepository(_dbContext);
        _accountValidation = new AccountValidation(_propertyValidations, _accountRepository);
        _cardRepository = new CardRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    private IAccountValidation _accountValidation;
    private IPropertyValidations _propertyValidations;
    private IAccountRepository _accountRepository;
    private CardRepository _cardRepository;
    private DbContextOptions<AppDbContext> _options;
    private AppDbContext _dbContext;

    [TestCase("invalid_iban")]
    [TestCase("1234567890123456789012345678901234567890123456789012345678901234")]
    [TestCase("EE001111222")]
    [TestCase("EE00111122222")]
    [TestCase("EE 0011112222")]
    [TestCase("EE001 1112222")]
    [TestCase("EE001111 2222")]
    [TestCase("EE001111222@")]
    [TestCase("EE001111222$")]
    public Task OnCreate_ThrowsException_WhenIbanIsInvalid(string iban)
    {
        var request = new CreateAccountRequest
        {
            Amount = 1000,
            Iban = iban,
            CurrencyCode = "EUR",
            PrivateNumber = "12345678901"
        };

        Assert.ThrowsAsync<Exception>(() => _accountValidation.OnCreate(request));
        return Task.CompletedTask;
    }

    [TestCase("01000000008")]
    [TestCase("01000000008")]
    [TestCase("01000000008")]
    [TestCase("01000000008")]
    public async Task OnCreate_ThrowsException_WhenPrivateNumberIsAlreadyUsed(string privateNumber)
    {
        var dbContext = new AppDbContext(_options);
        

        var request = new CreateAccountRequest
        {
            Amount = 1000,
            Iban = "EE0022223333",
            CurrencyCode = "EUR",
            PrivateNumber = privateNumber
        };
        
        await dbContext.Account.AddAsync(new AccountEntity
        {
            Iban = request.Iban,
            PrivateNumber = request.PrivateNumber,
            CurrencyCode = request.CurrencyCode,
            Balance = request.Amount
        });
        await dbContext.SaveChangesAsync();

        _accountValidation = new AccountValidation(_propertyValidations, _accountRepository);

        Assert.ThrowsAsync<Exception>(() => _accountValidation.OnCreate(request));
    }

    [TestCase("EE0011112222","12345678901")]
    [TestCase("EE1011112222","22345678901")]
    [TestCase("EE2011112222","32345678901")]
    [TestCase("EE3011112222","42345678901")]
    public async Task AccountWithIbanExists_Returns_True_When_Account_Exists(string iban, string privateNumber)
    {
        var account = new AccountEntity
        {
            Iban = iban,
            PrivateNumber = privateNumber,
            CurrencyCode = "Usd"
        };
        await _dbContext.Account.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        var result = await _accountValidation.AccountWithIbanExists(iban);

        Assert.That(result, Is.True);
    }
    
    [TestCase("EE0011112222", "EE0033334444", "USD", "USD", true)]
    [TestCase("EE0011112222", "EE0033334444", "USD", "EUR", false)]
    [TestCase("EE0011112222", "EE0033334444", "USD", "usd", true)]
    [TestCase("EE0011112222", "EE0033334444", "USD", "", false)]
    [TestCase("EE0011112222", "EE0033334444", "", "USD", false)]
    public async Task IsCurrencySame_Returns_Correct_Result(string aggressorIban, string receiverIban,
        string aggressorCurrencyCode, string receiverCurrencyCode, bool expectedResult)
    {
        var aggressorAccount = new AccountEntity
        {
            Iban = aggressorIban,
            PrivateNumber = "12345678901",
            CurrencyCode = aggressorCurrencyCode
        };
        var receiverAccount = new AccountEntity
        {
            Iban = receiverIban,
            PrivateNumber = "23456789012",
            CurrencyCode = receiverCurrencyCode
        };
        _dbContext.Account.AddRange(aggressorAccount, receiverAccount);
        await _dbContext.SaveChangesAsync();

        var result = await _accountValidation.IsCurrencySame(aggressorIban, receiverIban);

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task IsCurrencySame_Returns_CorrectResult()
    {
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

        Assert.That(await _accountValidation.IsCurrencySame(aggressorIban, receiverIban), Is.False);

        aggressorAccount.CurrencyCode = "EUR";
        _dbContext.Update(aggressorAccount);
        await _dbContext.SaveChangesAsync();

        Assert.That(await _accountValidation.IsCurrencySame(aggressorIban, receiverIban), Is.True);

        receiverAccount.CurrencyCode = "USD";
        _dbContext.Update(receiverAccount);
        await _dbContext.SaveChangesAsync();

        Assert.That(await _accountValidation.IsCurrencySame(aggressorIban, receiverIban), Is.False);
    }
    
    [TestCase("EE0011112222", 200)]
    [TestCase("EE0011112222", 150)]
    [TestCase("EE0011112222", 15000000)]
    public async Task HasSufficientBalance_Throws_Exception_When_Balance_Is_Less_Than_Amount(string iban,
        decimal amount)
    {
        var account = new AccountEntity
        {
            Iban = "EE0011112222",
            PrivateNumber = "12345678901",
            CurrencyCode = "USD",
            Balance = 100m
        };
        await _dbContext.Account.AddAsync(account);
        await _dbContext.SaveChangesAsync();

        Assert.ThrowsAsync<Exception>(async () =>
            await _accountValidation.HasSufficientBalance(iban, amount));
    }
    
    [TestCase("EE0011112222", 100.0)]
    [TestCase("EE0011223344", 0)]
    public async Task GetAmountWithIban_Returns_Correct_Balance(string iban, decimal expectedBalance)
    {
        var account = new AccountEntity
        {
            Iban = iban,
            PrivateNumber = "12345678901",
            CurrencyCode = "Usd",
            Balance = expectedBalance
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        var result = await _accountValidation.GetBalanceWithIban(iban);

        Assert.That(result, Is.EqualTo(expectedBalance));
    }


    [TestCase("EE0011112222")]
    [TestCase("EE0011223344")]
    [TestCase("EE0099887766")]
    public async Task GetAccountWithIban_Returns_Account_When_Exists(string iban)
    {
        var account = new AccountEntity
        {
            Iban = iban,
            PrivateNumber = "12345678901",
            CurrencyCode = "Usd",
            Balance = 100
        };
        _dbContext.Account.Add(account);
        await _dbContext.SaveChangesAsync();

        var result = await _accountRepository.GetAccountWithIban(iban);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<AccountEntity>());
    }

    [Test]
    public async Task HasTransaction_Returns_True_When_Transaction_Exists()
    {
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

        var result = await _accountRepository.HasTransaction("GB29NWBK60161331926821");

        Assert.That(result, Is.Not.Null);
    }

    [TestCase("GB29NWBK60161331926821", "1111222233334444", "John Doe", "123", "1234", "Usd", "1234567890")]
    public async Task GetCardWithIban_Returns_Card_When_Exists(string iban, string cardNumber, string cardHolderName,
        string cvv, string pin, string currencyCode, string privateNumber)
    {
        var card = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = cardNumber,
            CardHolderName = cardHolderName,
            Cvv = cvv,
            Pin = pin,
            ExpirationDate = new DateTime(2025, 12, 31),
            CreationDate = DateTime.UtcNow
        };
        var account = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = iban,
            Balance = 5000,
            CreationDate = DateTime.Now,
            CurrencyCode = currencyCode,
            PrivateNumber = privateNumber
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

        var result = await _cardRepository.GetCardWithIban(iban);

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