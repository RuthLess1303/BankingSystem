using InternetBank.Core.Services;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Models;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Core.Test.ServicesTests;

[TestFixture]
public class CardServiceTests
{
    private AppDbContext _dbContext;
    private ICardService _cardService;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureDeleted();
        _dbContext.Database.EnsureCreated();

        // _cardService = new CardService();
    }

    [TearDown]
    public void Teardown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task PrintCardModelProperties_ShouldReturnCorrectString()
    {
        // Arrange
        var card = new CardModel
        {
            CardNumber = "1234 5678 9012 3456",
            CardHolderName = "John Doe",
            Cvv = "123",
            ExpirationDate = new DateTime(2023, 12, 31)
        };

        // Act
        var result = _cardService.PrintCardModelProperties(card);

        // Assert
        Assert.That(result, Is.EqualTo("Card Number: 1234 5678 9012 3456\n" +
                                       "Name on Card: John Doe\n" +
                                       "Cvv: 123\n" +
                                       "Expiration Date: 31.12.2023 00:00:00\n"));
    }
}