// using InternetBank.Core.Services;
// using InternetBank.Core.Validations;
// using InternetBank.Db.Db;
// using InternetBank.Db.Db.Entities;
// using InternetBank.Db.Db.Repositories;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
//
// namespace InternetBank.Test.ServicesTests;
//
// [TestFixture]
// public class AccountServiceTests
// {
//     [SetUp]
//     public async Task SetUp()
//     {
//         _options = new DbContextOptionsBuilder<AppDbContext>()
//             .UseInMemoryDatabase("InternetBankTestDb")
//             .Options;
//
//         _dbContext = new AppDbContext(_options);
//         await _dbContext.Database.EnsureCreatedAsync();
//         
//         var account = new AccountEntity
//             { Iban = "NL12ABCD345678910", PrivateNumber = "1234567890", CurrencyCode = "GEL", Balance = 50 };
//         _dbContext.Account.Add(account);
//
//         var transaction1 = new TransactionEntity
//         {
//             SenderIban = account.Iban, ReceiverIban = "TR987654321098765432109876", Amount = 100,
//             TransactionTime = DateTime.UtcNow, CurrencyCode = "Usd", Type = "Transfer"
//         };
//         ;
//         var transaction2 = new TransactionEntity
//         {
//             SenderIban = "EE0011112222", ReceiverIban = account.Iban, Amount = 200, TransactionTime = DateTime.UtcNow,
//             CurrencyCode = "Gel", Type = "Transfer"
//         };
//         _dbContext.Transaction.Add(transaction1);
//         _dbContext.Transaction.Add(transaction2);
//
//         await _dbContext.SaveChangesAsync();
//
//
//         // Create an instance of the AccountService to be tested
//         var userStore = new UserStore<UserEntity, IdentityRole<int>, AppDbContext, int>(_dbContext);
//         var passwordHasher = new PasswordHasher<UserEntity>();
//         var userValidators = new List<IUserValidator<UserEntity>>();
//         var passwordValidators = new List<IPasswordValidator<UserEntity>>();
//         var keyNormalizer = new UpperInvariantLookupNormalizer();
//         var errors = new IdentityErrorDescriber();
//
//         var propertyValidations = new PropertyValidations(
//             new CurrencyRepository(_dbContext),
//             new UserRepository(_dbContext, new UserManager<UserEntity>(
//                 userStore,
//                 null, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, null, null)),
//             new AccountRepository(_dbContext),
//             new CardRepository(_dbContext)
//         );
//     }
//
//     [TearDown]
//     public void TearDown()
//     {
//         // Dispose the in-memory database
//         using var context = new AppDbContext(_options);
//         context.Database.EnsureDeleted();
//     }
//
//     private AccountService _accountService;
//     private DbContextOptions<AppDbContext> _options;
//     private AppDbContext _dbContext;
//     
//
//     [TestCase("GB12CDEF345678910")]
//     [TestCase("GE12CDEF882900129")]
//     [TestCase("GE12CD34225678910")]
//     public Task SeeAccount_ReturnsNull_WhenIbanDoesNotExist(string iban)
//     {
//         Assert.ThrowsAsync<Exception>(() => _accountService.SeeAccount(iban));
//         return Task.CompletedTask;
//     }
// }