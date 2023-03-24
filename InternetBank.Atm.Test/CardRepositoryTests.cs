using InternetBank.Atm.Core.Repositories;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Atm.Test;

[TestFixture]
public class CardRepositoryTests
{
    private AppDbContext _dbContext;
    private ICardRepository _cardRepository;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _cardRepository = new CardRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task FindCardEntityByCardNumberAsync_WhenCardExists_ReturnsCardEntity()
    {
        // Arrange
        const string cardNumber = "1234567890123456";
        var card = new CardEntity
        {
            CardNumber = cardNumber,
            Cvv = "123",
            NameOnCard = "John Doe",
            ExpirationDate = new DateTime(2025, 12, 31),
            CreationDate = DateTime.UtcNow,
            Pin = "012"
        };
        _dbContext.Card.Add(card);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cardRepository.FindCardEntityByCardNumberAsync(cardNumber);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CardNumber, Is.EqualTo(cardNumber));
    }

    [Test]
    public async Task FindCardEntityByCardNumberAsync_WhenCardDoesNotExist_ReturnsNull()
    {
        // Arrange
        const string cardNumber = "1234567890123456";

        // Act
        var result = await _cardRepository.FindCardEntityByCardNumberAsync(cardNumber);

        // Assert
        Assert.That(result, Is.Null);
    }
}