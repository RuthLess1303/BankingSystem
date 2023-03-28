using InternetBank.Core.Services;
using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Core.Test.ServicesTests
{
    [TestFixture]
    public class TransactionServiceTests
    {
        private AppDbContext _dbContext;
        private ITransactionRepository _transactionRepository;
        private ICurrencyService _currencyService;
        private IAccountValidation _accountValidation;
        private IAccountRepository _accountRepository;
        private ITransactionValidations _transactionValidations;
        private ITransactionService _transactionService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();

            _transactionRepository = new TransactionRepository(_dbContext);
            _currencyService = new CurrencyService(_dbContext,new CurrencyRepository(_dbContext));
            _accountValidation = new AccountValidation(
                new PropertyValidations(
                    new CurrencyRepository(_dbContext),
                    new UserRepository(_dbContext, new UserManager<UserEntity>(
                        new UserStore<UserEntity, IdentityRole<Guid>, AppDbContext, Guid>(_dbContext),
                        null, null, null, null, null, null, null, null)),
                    new AccountRepository(_dbContext),
                    new CardRepository(_dbContext)
                ),
                new AccountRepository(_dbContext));
            
            
            _accountRepository = new AccountRepository(_dbContext);
            _transactionValidations = new TransactionValidations();
            _transactionService = new TransactionService(
                new TransactionRepository(_dbContext),
                _currencyService,
                _accountValidation,
                new AccountRepository(_dbContext),
                _transactionValidations
            );
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task MakeTransaction_WhenTransactionIsSuccessful_ShouldAddTransactionToDatabase()
        {
            // Arrange
            var senderAccount = new AccountEntity
            {
                Id = Guid.NewGuid(),
                Iban = "EE0011112222",
                Balance = 5000,
                CreationDate = DateTime.Now,
                CurrencyCode = "USD",
                PrivateNumber = "1234567890"
            };
            var receiverAccount = new AccountEntity
            {
                Id = Guid.NewGuid(),
                Iban = "GG0011112222",
                Balance = 0,
                CreationDate = DateTime.Now,
                CurrencyCode = "USD",
                PrivateNumber = "9876543210"
            };

            var currency = new CurrencyEntity
            {
                Id = 1,
                Code = "USD",
                Quantity = 1,
                RateFormatted = 0.0000M,
                DiffFormatted = 0.0000M,
                Rate = 1.2M,
                Name = "აშშ დოლარი",
                Diff = -0.0004M,
                Date = DateTime.UtcNow.AddDays(-2),
                ValidFromDate = DateTime.UtcNow.AddDays(-1)
            };
            _dbContext.Currency.Add(currency);
            
            _dbContext.Account.AddRange(senderAccount, receiverAccount);
            await _dbContext.SaveChangesAsync();

            var transactionRequest = new TransactionRequest
            {
                SenderIban = senderAccount.Iban,
                ReceiverIban = receiverAccount.Iban,
                Amount = 500
            };

            // Act
            await _transactionService.MakeTransaction(transactionRequest);

            // Assert
            var transactions = await _dbContext.Transaction.ToListAsync();
            Assert.That(transactions, Has.Count.EqualTo(1));

            var transaction = transactions.First();
            Assert.Multiple(() =>
            {
                Assert.That(transaction.SenderIban, Is.EqualTo(transactionRequest.SenderIban));
                Assert.That(transaction.ReceiverIban, Is.EqualTo(transactionRequest.ReceiverIban));
                Assert.That(transaction.CurrencyCode, Is.EqualTo("USD"));
                Assert.That(transaction.Amount, Is.EqualTo(500));
                Assert.That(transaction.Rate, Is.EqualTo(1.2m));
                Assert.That(transaction.Fee, Is.EqualTo(5.5m));
                Assert.That(transaction.GrossAmount, Is.EqualTo(505.5m));
                Assert.That(transaction.Type, Is.EqualTo("Outside"));
            });
            Assert.NotNull(transaction.TransactionTime);
        }
    }
}
