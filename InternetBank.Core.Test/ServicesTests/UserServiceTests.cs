// using InternetBank.Core.Services;
// using InternetBank.Core.Validations;
// using InternetBank.Db.Db;
// using InternetBank.Db.Db.Entities;
// using InternetBank.Db.Db.Repositories;
// using InternetBank.Db.Requests;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Logging.Abstractions;
//
// namespace InternetBank.Core.Test.ServicesTests;
//
// [TestFixture]
// public class UserServiceTests
// {
//     // [SetUp]
// //     public async Task Setup()
// //     {
// //         var options = new DbContextOptionsBuilder<AppDbContext>()
// //             .UseInMemoryDatabase("TestDb")
// //             .Options;
// //
// //         _dbContext = new AppDbContext(options);
// //         await _dbContext.Database.EnsureDeletedAsync();
// //         await _dbContext.Database.EnsureCreatedAsync();
// //
// //
// //         // Create the UserManager to be used by the UserService and UserRepository
// //         _userManager = new UserManager<UserEntity>(
// //             new UserStore<UserEntity, IdentityRole<Guid>, AppDbContext, Guid>(_dbContext),
// //             null, null, null, null, null, null, null, null);
// //
// // // Create the repositories to be used by the UserService
// //         var userRepository = new UserRepository(_dbContext, _userManager);
// //         var accountRepository = new AccountRepository(_dbContext);
// //         var cardRepository = new CardRepository(_dbContext);
// //
// //
// // // Create the UserService instance to be tested
// //         var propertyValidations = new PropertyValidations(
// //             new CurrencyRepository(_dbContext),
// //             userRepository,
// //             accountRepository,
// //             cardRepository
// //         );
// //         _userService = new UserService(propertyValidations, userRepository, accountRepository, _userManager,
// //             cardRepository);
// //     }
//
//     [SetUp]
// public async Task Setup()
// {
//     var options = new DbContextOptionsBuilder<AppDbContext>()
//         .UseInMemoryDatabase(databaseName: "TestDb")
//         .Options;
//
//     _dbContext = new AppDbContext(options);
//     await _dbContext.Database.EnsureDeletedAsync();
//     await _dbContext.Database.EnsureCreatedAsync();
//
//     // Create the UserManager to be used by the UserService and UserRepository
//     var userStore = new UserStore<UserEntity, IdentityRole<Guid>, AppDbContext, Guid>(_dbContext);
//     var passwordHasher = new PasswordHasher<UserEntity>();
//     var userValidators = new List<IUserValidator<UserEntity>>();
//     var passwordValidators = new List<IPasswordValidator<UserEntity>>();
//     var keyNormalizer = new UpperInvariantLookupNormalizer();
//     var errors = new IdentityErrorDescriber();
//     var services = new ServiceCollection();
//     services.AddSingleton<ILogger<UserManager<UserEntity>>>(NullLogger<UserManager<UserEntity>>.Instance);
//
//     services.AddScoped<IRoleStore<IdentityRole<Guid>>, RoleStore<IdentityRole<Guid>, AppDbContext, Guid>>();
//     services.AddScoped<RoleManager<IdentityRole<Guid>>>();
//
//     var userManager = new UserManager<UserEntity>(
//         userStore, optionsAccessor: null, passwordHasher,
//         userValidators, passwordValidators, keyNormalizer, errors, null, null);
//
//     userManager.UserValidators.Clear();
//     userManager.UserValidators.Add(new UserValidator<UserEntity>());
//
//     userManager.PasswordValidators.Clear();
//     userManager.PasswordValidators.Add(new PasswordValidator<UserEntity>());
//
//     var roleManager = services.BuildServiceProvider().GetRequiredService<RoleManager<IdentityRole<Guid>>>();
//     await roleManager.CreateAsync(new IdentityRole<Guid>("RoleName"));
//
//     var user = new UserEntity
//     {
//         FirstName = "John",
//         LastName = "Doe",
//         Email = "john.doe@example.com",
//         PrivateNumber = "12345678901",
//         UserName = "john.doe@example.com"
//     };
//
//     await userManager.CreateAsync(user, "Pa$$w0rd");
//
//     await userManager.AddToRoleAsync(user, "RoleName");
//
//     _userManager = userManager;
//
//     // Create the repositories to be used by the UserService
//     var userRepository = new UserRepository(_dbContext, userManager);
//     var accountRepository = new AccountRepository(_dbContext);
//     var cardRepository = new CardRepository(_dbContext);
//
//             // Create the UserService instance to be tested
//             var propertyValidations = new PropertyValidations(
//                 new CurrencyRepository(_dbContext),
//                 userRepository,
//                 accountRepository,
//                 cardRepository
//             );
//             _userService = new UserService(propertyValidations, userRepository, accountRepository, userManager, cardRepository);
//         }
//     
//     [TearDown]
//     public void Cleanup()
//     {
//         // Dispose of the DbContext and close the in-memory database connection
//         _dbContext.Dispose();
//     }
//
//     private IUserService _userService;
//     private UserManager<UserEntity> _userManager;
//     private AppDbContext _dbContext;
//
//     [Test]
//     public async Task Register_ValidUser_Success()
//     {
//         // Arrange
//         var request = new RegisterUserRequest
//         {
//             Name = "John",
//             Surname = "Doe",
//             Email = "john.doe@example.com",
//             Password = "Pa$$w0rd",
//             PrivateNumber = "12345678901"
//         };
//
//         // Act
//         await _userService.Register(request);
//
//         // Assert
//         var user = await _userManager.FindByEmailAsync(request.Email);
//         Assert.NotNull(user);
//         Assert.AreEqual(request.Name, user.FirstName);
//         Assert.AreEqual(request.Surname, user.LastName);
//         Assert.AreEqual(request.PrivateNumber, user.PrivateNumber);
//     }
//
//     // Add more tests for other methods of the UserService class
// }


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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace InternetBank.Core.Test.ServicesTests;

[TestFixture]
public class UserServiceTests
{
    private AppDbContext _dbContext;
    private IUserRepository _userRepository;
    private UserManager<UserEntity> _userManager;
    private IPropertyValidations _propertyValidations;
    private IUserService _userService;
    
    [SetUp]
    public async Task SetUp()
    {
        // // Set up an in-memory database for testing
        // var options = new DbContextOptionsBuilder<AppDbContext>()
        //     .UseInMemoryDatabase("InternetBank")
        //     .Options;
        // _dbContext = new AppDbContext(options);
        // await _dbContext.Database.EnsureCreatedAsync();
        //
        //
        // // Set up the user manager
        // var userStore = new UserStore<UserEntity, IdentityRole<int>, AppDbContext, int>(_dbContext);
        // var passwordHasher = new PasswordHasher<UserEntity>();
        // var userValidators = new List<IUserValidator<UserEntity>>();
        // var passwordValidators = new List<IPasswordValidator<UserEntity>>();
        // var keyNormalizer = new UpperInvariantLookupNormalizer();
        // var errors = new IdentityErrorDescriber();
        // var services = new ServiceCollection();
        // services.AddSingleton<ILogger<UserManager<UserEntity>>>(NullLogger<UserManager<UserEntity>>.Instance);
        //
        // services.AddScoped<IRoleStore<IdentityRole<Guid>>, RoleStore<IdentityRole<Guid>, AppDbContext, Guid>>();
        // services.AddScoped<RoleManager<IdentityRole<Guid>>>();
        // _userManager = new UserManager<UserEntity>(userStore, null, passwordHasher, userValidators, passwordValidators,
        //     keyNormalizer, errors, null, null);
        //
        // // Set up the user service
        // var propertyValidations = new PropertyValidations(
        //     new CurrencyRepository(_dbContext),
        //     new UserRepository(_dbContext, new UserManager<UserEntity>(
        //         userStore,
        //         null, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, null, null)),
        //     new AccountRepository(_dbContext),
        //     new CardRepository(_dbContext)
        // );
        // var userRepository = new UserRepository(_dbContext, _userManager);
        // var accountRepository = new AccountRepository(_dbContext);
        // var cardRepository = new CardRepository(_dbContext);
        //  _userService = new UserService(propertyValidations, userRepository, _userManager);
        //  
         
         
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "InternetBank")
            .Options;
        _dbContext = new AppDbContext(options);
        
        
        var userStore = new UserStore<UserEntity, IdentityRole<int>, AppDbContext, int>(_dbContext);
        var passwordHasher = new PasswordHasher<UserEntity>();
        var userValidators = new List<IUserValidator<UserEntity>>();
        var passwordValidators = new List<IPasswordValidator<UserEntity>>();
        var keyNormalizer = new UpperInvariantLookupNormalizer();
        var errors = new IdentityErrorDescriber();
        
        _userManager = new UserManager<UserEntity>(userStore, null, passwordHasher, userValidators, passwordValidators,
            keyNormalizer, errors, null, null);
        var propertyValidations = new PropertyValidations(
            new CurrencyRepository(_dbContext),
            new UserRepository(_dbContext, _userManager),
            new AccountRepository(_dbContext),
            new CardRepository(_dbContext)
        );
        _userRepository = new UserRepository(_dbContext,_userManager);
        _userService = new UserService(propertyValidations, _userRepository, _userManager);
    }

    [TearDown]
    public async Task TearDown()
    {
        // Clean up the in-memory database after each test
        await _dbContext.Database.EnsureDeletedAsync();
    }

    [Test]
    public async Task Register_Should_Register_New_User()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            PrivateNumber = "12345678901",
            Name = "John",
            Surname = "Doe",
            Email = "johndoe@example.com",
            Password = "Password123!"
        };

        // Act
        await _userService.Register(request);

        // Assert
        // var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        var user = await _userRepository.FindWithEmail(request.Email);
        Assert.That(user, Is.Not.Null);
        Assert.AreEqual(request.PrivateNumber, user.PrivateNumber);
        Assert.AreEqual(request.Name, user.FirstName);
        Assert.AreEqual(request.Surname, user.LastName);
    }
    
}