using BankingSystem.Atm.Core.Repositories;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test;

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
        const string cardNumber = "1234567890123456";
        var card = new CardEntity
        {
            CardNumber = cardNumber,
            Cvv = "123",
            CardHolderName = "John Doe",
            ExpirationDate = new DateTime(2025, 12, 31),
            CreationDate = DateTime.UtcNow,
            Pin = "012"
        };
        _dbContext.Card.Add(card);
        await _dbContext.SaveChangesAsync();

        var result = await _cardRepository.FindCardEntityByCardNumberAsync(cardNumber);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.CardNumber, Is.EqualTo(cardNumber));
    }

    [Test]
    public Task FindCardEntityByCardNumberAsync_WhenCardDoesNotExist_ReturnsNull()
    {
        const string cardNumber = "1234567890123456";
        
        Assert.ThrowsAsync<ArgumentException>(() => _cardRepository.FindCardEntityByCardNumberAsync(cardNumber),
            $"Card with number {cardNumber} does not exist");
        return Task.CompletedTask;
    }
}