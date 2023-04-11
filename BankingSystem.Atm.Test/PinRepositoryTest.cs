using BankingSystem.Atm.Core.Repositories;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Test;

[TestFixture]
public class PinRepositoryTests
{
    private AppDbContext _dbContext;
    private IPinRepository _pinRepository;
    
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _pinRepository = new CardPinRepository(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
    
    [Test]
    public async Task ChangePinInDb_WhenCalled_UpdatesPinInDatabase()
    {
        var card = new CardEntity
        {
            CardNumber = "1234567890123456",
            Cvv = "123",
            CardHolderName = "John Doe",
            ExpirationDate = new DateTime(2025, 12, 31),
            CreationDate = DateTime.UtcNow,
            Pin = "1234"
        };
        _dbContext.Card.Add(card);
        await _dbContext.SaveChangesAsync();

        const string newPin = "5678";

        await _pinRepository.ChangePinInDb(card, newPin);

        var updatedCard = await _dbContext.Card.FindAsync(card.Id);
        Assert.That(updatedCard, Is.Not.Null);
        Assert.That(updatedCard.Pin, Is.EqualTo(newPin));
    }
}