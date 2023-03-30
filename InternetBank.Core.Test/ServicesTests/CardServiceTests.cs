using InternetBank.Core.Services;
using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Models;
using InternetBank.Db.Db.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

        var userStore = new UserStore<UserEntity, IdentityRole<int>, AppDbContext, int>(_dbContext);
        var passwordHasher = new PasswordHasher<UserEntity>();
        var userValidators = new List<IUserValidator<UserEntity>>();
        var passwordValidators = new List<IPasswordValidator<UserEntity>>();
        var keyNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        var userRepository = new UserRepository(_dbContext, new UserManager<UserEntity>(
                    userStore,
                    null, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, null, null));
        
        var propertyValidations = new PropertyValidations(
            new CurrencyRepository(_dbContext),
            userRepository,
            new AccountRepository(_dbContext),
            new CardRepository(_dbContext)
        );
        
        
        
         _cardService = new CardService(new CardValidation(propertyValidations,new CardRepository(_dbContext)),propertyValidations,new AccountRepository(_dbContext),new CardRepository(_dbContext),userRepository);
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