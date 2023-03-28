using InternetBank.Core.Services;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Reporting.Core.Repositories;
using InternetBank.Reporting.Core.Validations;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Reporting.Test
{
    public class TransactionStatisticsRepositoryTests
    {
        private DbContextOptions<AppDbContext> _options;
        private AppDbContext _context;
        private TransactionStatisticsRepository _transactionStatisticsRepository;
        private TransactionStatisticsValidations _transactionStatisticsValidations;
        private CurrencyService _currencyService;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new AppDbContext(_options);
            _transactionStatisticsRepository = new TransactionStatisticsRepository(_context);
            _currencyService = new CurrencyService(_context, new CurrencyRepository(_context));
            _transactionStatisticsValidations = new TransactionStatisticsValidations(_transactionStatisticsRepository,_currencyService,new AccountRepository(_context));

            // SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // private void SeedData()
        // {
        //     // Seed some test data
        //     var innerTransactions = new List<TransactionEntity>
        //     {
        //         new() { Id = 1, Amount = 100, CurrencyCode = "GEL", Fee = 10 },
        //         new() { Id = 2, Amount = 50, CurrencyCode = "USD", Fee = 5 },
        //         new() { Id = 3, Amount = 200, CurrencyCode = "EUR", Fee = 20 }
        //     };
        //
        //     var outsideTransactions = new List<TransactionEntity>
        //     {
        //         new() { Id = 4, Amount = 150, CurrencyCode = "GEL", Fee = 15 },
        //         new() { Id = 5, Amount = 75, CurrencyCode = "USD", Fee = 7.5m },
        //         new() { Id = 6, Amount = 300, CurrencyCode = "EUR", Fee = 30 }
        //     };
        //
        //     _context.Transaction.AddRange(innerTransactions);
        //     _context.Transaction.AddRange(outsideTransactions);
        //     _context.SaveChanges();
        // }

        [Test]
        public async Task TotalIncomeFromTransactionsByMonth_ReturnsCorrectTotalIncome()
        {

            // Arrange
            var currencyList = new[]
            {
                new CurrencyEntity
                {
                    Id = 3,
                    Code = "GEL",
                    Quantity = 1,
                    RateFormatted = 0.0000M,
                    DiffFormatted = 0.0000M,
                    Rate = 1M,
                    Name = "ლარი",
                    Diff = 1M,
                    Date = DateTime.UtcNow.AddDays(-2),
                    ValidFromDate = DateTime.UtcNow.AddDays(-1)
                },
                new CurrencyEntity
                {
                    Id = 1,
                    Code = "USD",
                    Quantity = 1,
                    RateFormatted = 0.0000M,
                    DiffFormatted = 0.0000M,
                    Rate = 2.5764M,
                    Name = "აშშ დოლარი",
                    Diff = -0.0004M,
                    Date = DateTime.UtcNow.AddDays(-2),
                    ValidFromDate = DateTime.UtcNow.AddDays(-1)
                },
                new CurrencyEntity
                {
                    Id = 2,
                    Code = "EUR",
                    Quantity = 1,
                    RateFormatted = 0.0000M,
                    DiffFormatted = 0.0000M,
                    Rate = 2.7665M,
                    Name = "ევრო",
                    Diff = -0.0363M,
                    Date = DateTime.UtcNow.AddDays(-2),
                    ValidFromDate = DateTime.UtcNow.AddDays(-1)
                }
            };

            // Act
            await _context.Currency.AddRangeAsync(currencyList);
            
            
            // Seed test data
            _context.Transaction.Add(new TransactionEntity
                { Id = 1, Amount = 100, CurrencyCode = "USD", Fee = 10, Type = "Inner",SenderIban = "GE40759994372495896628",ReceiverIban = "BE55812755263844",TransactionTime = new DateTime(2023, 3, 22, 14, 30, 0)});
            _context.Transaction.Add(new TransactionEntity
                { Id = 2, Amount = 50, CurrencyCode = "USD", Fee = 5, Type = "Inner",ReceiverIban = "GE40759994372495896628",SenderIban = "BE55812755263844",TransactionTime = new DateTime(2023, 3, 22, 14, 30, 0)});
            _context.Transaction.Add(new TransactionEntity
                { Id = 3, Amount = 200, CurrencyCode = "EUR", Fee = 20, Type = "Outside",SenderIban = "NL07RABO5528005485",ReceiverIban = "DK2350517488741492",TransactionTime = new DateTime(2023, 3, 22, 14, 30, 0)});
            _context.Transaction.Add(new TransactionEntity
                { Id = 4, Amount = 100, CurrencyCode = "EUR", Fee = 10, Type = "Outside" , ReceiverIban = "NL07RABO5528005485",SenderIban = "DK2350517488741492",TransactionTime = new DateTime(2023, 3, 22, 14, 30, 0)});
            await _context.SaveChangesAsync();

            // Arrange
            const decimal expectedGelInnerIncome = 15m;
            const decimal expectedGelOutsideIncome = 30m;
            var expectedGelTotalIncome = expectedGelInnerIncome + expectedGelOutsideIncome;

            // Act
            var (actualGelInnerIncome, actualGelOutsideIncome, actualGelTotalIncome) = await 
                _transactionStatisticsValidations.TotalIncomeFromTransactionsByMonth(1);

            // Assert
            Assert.That(actualGelInnerIncome, Is.EqualTo(expectedGelInnerIncome));
            Assert.That(actualGelOutsideIncome, Is.EqualTo(expectedGelOutsideIncome));
            Assert.That(actualGelTotalIncome, Is.EqualTo(expectedGelTotalIncome));
        }

        [Test]
        public void TotalIncomeFromTransactionsByMonth_ThrowsExceptionWhenInnerTransactionsNotFound()
        {
            // Arrange
            _context.Transaction.RemoveRange(_context.Transaction);
            _context.SaveChanges();

            // Act + Assert
            Assert.ThrowsAsync<Exception>(async () => await _transactionStatisticsValidations.TotalIncomeFromTransactionsByMonth(1));
        }

        [Test]
        public void TotalIncomeFromTransactionsByMonth_ThrowsExceptionWhenOutsideTransactionsNotFound()
        {
            // Arrange
            _context.Transaction.RemoveRange(_context.Transaction.Where(t => t.CurrencyCode != "GEL"));
            _context.SaveChanges();

            // Act + Assert
            Assert.ThrowsAsync<Exception>(async () => await _transactionStatisticsValidations.TotalIncomeFromTransactionsByMonth(1));
        }
    }
}
