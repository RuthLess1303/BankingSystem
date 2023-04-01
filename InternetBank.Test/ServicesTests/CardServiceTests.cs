using InternetBank.Core.Services;
using InternetBank.Core.Validations;
using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Models;
using InternetBank.Db.Db.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Test.ServicesTests;

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
    
    [TestCase("1234 5678 9012 3456", "John Doe", "123")]
    [TestCase("0987 6543 2109 8765", "Mike Tyson", "000")]
    [TestCase("0092 1234 0982 9901", "Joe Mamba", "001")]
    [TestCase("1029 2001 2345 2999", "John Doe", "111")]
    public async Task PrintCardModelProperties_ShouldReturnCorrectString(string cardNumber, string cadHolderName,
        string cvv)
    {
        var card = new CardModel
        {
            CardNumber = cardNumber,
            CardHolderName = cadHolderName,
            Cvv = cvv,
            ExpirationDate = new DateTime(2023, 12, 31)
        };
        var result = _cardService.PrintCardModelProperties(card);
        Assert.That(result, Is.EqualTo($"Card Number: {cardNumber}\n" +
                                       $"Name on Card: {cadHolderName}\n" +
                                       $"Cvv: {cvv}\n" +
                                       $"Expiration Date: {card.ExpirationDate}\n"));
    }
}