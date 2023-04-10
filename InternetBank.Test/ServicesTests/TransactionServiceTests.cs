using InternetBank.Core.Services;
using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InternetBank.Test.ServicesTests;

[TestFixture]
public class TransactionServiceTests
{
    private AppDbContext? _dbContext;
    private ITransactionRepository? _transactionRepository;
    private ICurrencyService? _currencyService;
    private IAccountValidation? _accountValidation;
    private IAccountRepository? _accountRepository;
    private ITransactionValidations? _transactionValidations;
    private ITransactionService? _transactionService;
    private ICurrentUserValidation? _currentUserValidation;
    private IUserRepository? _userRepository;
    private ICurrencyRepository? _currencyRepository;
    
    [SetUp]
    public void Setup()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .UseInternalServiceProvider(serviceProvider)
            .Options;

        _dbContext = new AppDbContext(options);
        _accountRepository = new AccountRepository(_dbContext);

        var userStore = new UserStore<UserEntity, IdentityRole<int>, AppDbContext, int>(_dbContext);
        var passwordHasher = new PasswordHasher<UserEntity>();
        var userValidators = new List<IUserValidator<UserEntity>> { new UserValidator<UserEntity>() };
        var passwordValidators = new List<IPasswordValidator<UserEntity>> { new PasswordValidator<UserEntity>() };
        var keyNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        _userRepository = new UserRepository(_dbContext, new UserManager<UserEntity>(
            userStore,
            null, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, null, null));

        _currencyRepository = new CurrencyRepository(_dbContext);
        _accountValidation = new AccountValidation(
            new PropertyValidations(
                _currencyRepository,
                _userRepository,
                _accountRepository
            ),
            _accountRepository);

        _transactionRepository = new TransactionRepository(_dbContext);
        _transactionValidations = new TransactionValidations();
        _currentUserValidation =
            new CurrentUserValidation(_userRepository, new LoginLoggerRepository(_dbContext));
        _currencyService = new CurrencyService(_currencyRepository);
        _transactionService = new TransactionService(
            _transactionRepository,
            _currencyService,
            _accountValidation,
            _transactionValidations,
            _userRepository,
            _currentUserValidation
        );
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    [TestCase("GEL", "USD", 1000, 0, 500, "Outside",
        494.5, 197.62, 197.62)]
    [TestCase("USD", "USD", 1000, 0, 500, "Outside", 494.5, 500, 500)]
    public async Task MakeExternalTransaction_WhenTransactionIsSuccessful_ShouldAddTransactionToDatabase(
        string senderCurrencyCode,
        string receiverCurrencyCode,
        decimal senderBalance,
        decimal receiverBalance,
        decimal amount,
        string type,
        decimal senderAmountAfterTransaction,
        decimal receiverAmountAfterTransaction,
        decimal convertedAmount
    )
    {
        var senderUser = new UserEntity
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            PrivateNumber = "1234567890",
            BirthDate = new DateTime(1990, 1, 1),
            CreationDate = DateTime.UtcNow
        };
        var receiverUser = new UserEntity
        {
            Email = "john.doe@example.com",
            FirstName = "Nikol",
            LastName = "Doe",
            PrivateNumber = "9876543210",
            BirthDate = new DateTime(1999, 1, 9),
            CreationDate = DateTime.UtcNow
        };
        var senderAccount = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "GE48NB0000000042569287",
            Balance = senderBalance,
            CreationDate = DateTime.Now,
            CurrencyCode = senderCurrencyCode,
            PrivateNumber = "1234567890"
        };
        var receiverAccount = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "GE33CD0000000013137993",
            Balance = receiverBalance,
            CreationDate = DateTime.Now,
            CurrencyCode = receiverCurrencyCode,
            PrivateNumber = "9876543210"
        };

        var currency1 = new CurrencyEntity
        {
            Id = 1,
            Code = "USD",
            Quantity = 1,
            RateFormatted = 0.0000M,
            DiffFormatted = 0.0000M,
            Rate = 2.53M,
            Name = "აშშ დოლარი",
            Diff = -0.004M,
            Date = DateTimeOffset.UtcNow.AddDays(-2),
            ValidFromDate = DateTimeOffset.UtcNow.AddDays(-1),
            RatePerQuantity = 2.53M
        };
        var currency2 = new CurrencyEntity
        {
            Id = 2,
            Code = "GEL",
            Quantity = 1,
            RateFormatted = 0.0000M,
            DiffFormatted = 0.0000M,
            Rate = 1M,
            Name = "ლარი",
            Diff = -0.004M,
            Date = DateTimeOffset.UtcNow.AddDays(-2),
            ValidFromDate = DateTimeOffset.UtcNow.AddDays(-1),
            RatePerQuantity = 1
        };
        var userLogins = new UserLoginsEntity
        {
            Id = 1,
            UserId = 1,
            LoginDate = DateTimeOffset.Now
        };
        await _dbContext.UserLogins.AddAsync(userLogins);
        await _dbContext.Currency.AddRangeAsync(currency1, currency2);
        await _dbContext.Users.AddRangeAsync(senderUser, receiverUser);
        await _dbContext.Account.AddRangeAsync(senderAccount, receiverAccount);
        await _dbContext.SaveChangesAsync();

        var transactionRequest = new TransactionRequest
        {
            SenderIban = senderAccount.Iban,
            ReceiverIban = receiverAccount.Iban,
            Amount = amount
        };
        await _transactionService.MakeTransaction(transactionRequest);

        var transaction = await _dbContext.Transaction.FirstOrDefaultAsync();

        // Assert
        Assert.That(transaction, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(transaction.SenderIban, Is.EqualTo(senderAccount.Iban));
            Assert.That(transaction.ReceiverIban, Is.EqualTo(receiverAccount.Iban));
            Assert.That(transaction.CurrencyCode, Is.EqualTo(receiverCurrencyCode));
            Assert.That(transaction.Amount, Is.EqualTo(convertedAmount).Within(0.01m));
            Assert.That(transaction.Type, Is.EqualTo(type));
            Assert.That(transaction.TransactionTime.Date, Is.EqualTo(DateTimeOffset.Now.Date));
        });

        var senderAccountAfterTransaction = _dbContext.Account.FirstOrDefault(a => a.Id == senderAccount.Id);
        var receiverAccountAfterTransaction = _dbContext.Account.FirstOrDefault(a => a.Id == receiverAccount.Id);
        Assert.Multiple(() =>
        {
            Assert.That(senderAccountAfterTransaction, Is.Not.Null);
            Assert.That(receiverAccountAfterTransaction, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(senderAccountAfterTransaction.Balance, Is.EqualTo(senderAmountAfterTransaction));
            Assert.That(receiverAccountAfterTransaction.Balance,
                Is.EqualTo(receiverAmountAfterTransaction).Within(0.01m));
        });
    }

    [Test]
    [TestCase("GEL", "GEL", 1000, 0, 500,
        500, 500, 500)]
    [TestCase("GEL", "USD", 1000, 0, 500,
        500, 197.62, 197.62)]
    public async Task MakeInternalTransaction_WhenTransactionIsSuccessful_ShouldAddTransactionToDatabase(
        string senderCurrencyCode,
        string receiverCurrencyCode,
        decimal senderBalance,
        decimal receiverBalance,
        decimal amount,
        decimal senderAmountAfterTransaction,
        decimal receiverAmountAfterTransaction,
        decimal convertedAmount
    )
    {
        var senderUser = new UserEntity
        {
            Email = "john.doe@example.com",
            FirstName = "John",
            LastName = "Doe",
            PrivateNumber = "1234567890",
            BirthDate = new DateTime(1990, 1, 1),
            CreationDate = DateTime.UtcNow
        };
        var senderAccount = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "GE48NB0000000042569287",
            Balance = senderBalance,
            CreationDate = DateTime.Now,
            CurrencyCode = senderCurrencyCode,
            PrivateNumber = "1234567890"
        };
        var receiverAccount = new AccountEntity
        {
            Id = Guid.NewGuid(),
            Iban = "GE33CD0000000013137993",
            Balance = receiverBalance,
            CreationDate = DateTime.Now,
            CurrencyCode = receiverCurrencyCode,
            PrivateNumber = "1234567890"
        };

        var currency1 = new CurrencyEntity
        {
            Id = 1,
            Code = "USD",
            Quantity = 1,
            RateFormatted = 0.0000M,
            DiffFormatted = 0.0000M,
            Rate = 2.53M,
            Name = "აშშ დოლარი",
            Diff = -0.004M,
            Date = DateTimeOffset.UtcNow.AddDays(-2),
            ValidFromDate = DateTimeOffset.UtcNow.AddDays(-1),
            RatePerQuantity = 2.53M
        };
        var currency2 = new CurrencyEntity
        {
            Id = 2,
            Code = "GEL",
            Quantity = 1,
            RateFormatted = 0.0000M,
            DiffFormatted = 0.0000M,
            Rate = 1M,
            Name = "ლარი",
            Diff = -0.004M,
            Date = DateTimeOffset.UtcNow.AddDays(-2),
            ValidFromDate = DateTimeOffset.UtcNow.AddDays(-1),
            RatePerQuantity = 1
        };
        var userLogins = new UserLoginsEntity
        {
            Id = 1,
            UserId = 1,
            LoginDate = DateTimeOffset.Now
        };
        await _dbContext.UserLogins.AddAsync(userLogins);
        await _dbContext.Currency.AddRangeAsync(currency1, currency2);
        await _dbContext.Users.AddAsync(senderUser);
        await _dbContext.Account.AddRangeAsync(senderAccount, receiverAccount);
        await _dbContext.SaveChangesAsync();

        var transactionRequest = new TransactionRequest
        {
            SenderIban = senderAccount.Iban,
            ReceiverIban = receiverAccount.Iban,
            Amount = amount
        };
        await _transactionService.MakeTransaction(transactionRequest);

        var transaction = await _dbContext.Transaction.FirstOrDefaultAsync();

        // Assert
        Assert.That(transaction, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(transaction.SenderIban, Is.EqualTo(senderAccount.Iban));
            Assert.That(transaction.ReceiverIban, Is.EqualTo(receiverAccount.Iban));
            Assert.That(transaction.CurrencyCode, Is.EqualTo(receiverCurrencyCode));
            Assert.That(transaction.Amount, Is.EqualTo(convertedAmount).Within(0.01m));
            Assert.That(transaction.Type, Is.EqualTo("Inner"));
            Assert.That(transaction.TransactionTime.Date, Is.EqualTo(DateTimeOffset.Now.Date));
        });

        var senderAccountAfterTransaction = _dbContext.Account.FirstOrDefault(a => a.Id == senderAccount.Id);
        var receiverAccountAfterTransaction = _dbContext.Account.FirstOrDefault(a => a.Id == receiverAccount.Id);
        Assert.Multiple(() =>
        {
            Assert.That(senderAccountAfterTransaction, Is.Not.Null);
            Assert.That(receiverAccountAfterTransaction, Is.Not.Null);
        });
        Assert.Multiple(() =>
        {
            Assert.That(senderAccountAfterTransaction.Balance, Is.EqualTo(senderAmountAfterTransaction));
            Assert.That(receiverAccountAfterTransaction.Balance,
                Is.EqualTo(receiverAmountAfterTransaction).Within(0.01m));
        });
    }
}