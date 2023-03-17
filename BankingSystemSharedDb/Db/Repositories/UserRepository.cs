using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db.Repositories;

public interface IUserRepository
{
    Task<AccountEntity> GetAccountByCardDetails(string cardNumber, int pin);
    Task<UserEntity> GetUserByCardDetails(string cardNumber, int pin);
    Task<UserEntity?> FindWithPrivateNumber(string privateNumber);
    Task<UserEntity?> FindWithId(int id);
    Task<UserEntity?> FindWithEmail(string email);
    Task Register(RegisterUserRequest request);
    Task CreateCard(CardEntity cardEntity);
    Task<UserEntity?> GetUserWithEmail(string email);
    Task<UserEntity?> GetOperatorWithEmail(string email);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    private readonly UserManager<UserEntity> _userManager;

    public UserRepository(AppDbContext db, UserManager<UserEntity> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<UserEntity?> FindWithPrivateNumber(string privateNumber)
    {
        return await _db.User.FirstOrDefaultAsync(u => u.PrivateNumber == privateNumber);
    }

    public async Task<UserEntity?> FindWithId(int id)
    {
        return await _db.User.FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<UserEntity?> FindWithEmail(string email)
    {
        return await _db.User.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task Register(RegisterUserRequest request)
    {
        var user = new UserEntity
        {
            FirstName = request.Name,
            LastName = request.Surname,
            PrivateNumber = request.PrivateNumber,
            UserName = request.Email,
            Email = request.Email,
            BirthDate = request.BirthDate,
            CreationDate = DateTime.Now
        };
        
        var hasher = new PasswordHasher<UserEntity>();
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        await _userManager.CreateAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, "api-user");
    }
    
    public async Task CreateCard(CardEntity cardEntity)
    {
        await _db.AddAsync(cardEntity);
        await _db.SaveChangesAsync();

        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var user = await GetUserByCardDetails(cardEntity.CardNumber, cardEntity.Pin);
            var account = await GetAccountByCardDetails(cardEntity.CardNumber, cardEntity.Pin);

            account.Cards.Add(cardEntity);
            user.Cards.Add(cardEntity);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<UserEntity?> GetUserWithEmail(string email)
    {
        var user = await Task.Run(() => _db.User.FirstOrDefault(u => u.Email == email));

        return user;
    }
    
    public async Task<UserEntity?> GetOperatorWithEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<AccountEntity> GetAccountByCardDetails(string cardNumber, int pin)
    {
        var card = await _db.Card.FirstOrDefaultAsync(c => c.CardNumber == cardNumber && c.Pin == pin);
        if (card == null)
        {
            throw new UnauthorizedAccessException("Invalid card number or PIN code");
        }

        var cardAccountConnection = await _db.CardAccountConnection.FirstOrDefaultAsync(c => c.CardId == card.Id);
        if (cardAccountConnection == null)
        {
            throw new Exception("No account found for the card");
        }

        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == cardAccountConnection.Iban);
        if (account == null)
        {
            throw new Exception("No account found for the card");
        }

        return account;
    }
    
    public async Task<UserEntity> GetUserByCardDetails(string cardNumber, int pin)
    {
        var card = await _db.Card.FirstOrDefaultAsync(c => c.CardNumber == cardNumber && c.Pin == pin);
        if (card == null)
        {
            throw new UnauthorizedAccessException("Invalid card number or PIN code");
        }

        var cardAccountConnection = await _db.CardAccountConnection.FirstOrDefaultAsync(c => c.CardId == card.Id);
        if (cardAccountConnection == null)
        {
            throw new Exception("No account found for the card");
        }

        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == cardAccountConnection.Iban);
        if (account == null)
        {
            throw new Exception("No account found for the card");
        }

        var user = await _db.User.FirstOrDefaultAsync(u => u.Id == account.user.Id);
        if (user == null)
        {
            throw new Exception("No user found for the account");
        }

        return user;
    }
}