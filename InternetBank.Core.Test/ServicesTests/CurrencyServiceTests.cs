using InternetBank.Core.Services;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Core.Test.ServicesTests;

[TestFixture]
public class CurrencyServiceTests
{
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb0")
            .Options;

        _dbContext = new AppDbContext(options);
        _currencyRepository = new CurrencyRepository(_dbContext);
        _currencyService = new CurrencyService(_dbContext, _currencyRepository);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    private AppDbContext _dbContext;
    private ICurrencyRepository _currencyRepository;
    private ICurrencyService _currencyService;


    [Test]
    public async Task AddInDb_AddsCurrenciesToDatabase()
    {
        // Arrange
        var currencyList = new[]
        {
            new CurrencyEntity
            {
                Id = 1,
                Code = "USD",
                Quantity = 1,
                RateFormatted = 0.0000M,
                DiffFormatted = 0.0000M,
                Rate = 2.5754M,
                Name = "აშშ დოლარი",
                Diff = -0.001M,
                Date = DateTime.UtcNow.AddDays(0),
                ValidFromDate = DateTime.UtcNow.AddDays(1)
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
                Date = DateTime.UtcNow.AddDays(0),
                ValidFromDate = DateTime.UtcNow.AddDays(1)
            }
        };

        // Act
        await _currencyService.AddInDb();

        // Assert
        foreach (var currency in currencyList)
        {
            var result = await _dbContext.Currency.FirstOrDefaultAsync(c => c.Code == currency.Code);
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result.Code, Is.TypeOf<string>());
                Assert.That(result.Quantity, Is.TypeOf<int>());
                Assert.That(result.RateFormatted, Is.TypeOf<decimal>());
                Assert.That(result.DiffFormatted, Is.TypeOf<decimal>());
                Assert.That(result.Rate, Is.TypeOf<decimal>());
                Assert.That(result.Name, Is.TypeOf<string>());
                Assert.That(result.Diff, Is.TypeOf<decimal>());
                Assert.That(result.Date, Is.TypeOf<DateTime>());
                Assert.That(result.ValidFromDate, Is.TypeOf<DateTime>());
            });
        }
    }
}