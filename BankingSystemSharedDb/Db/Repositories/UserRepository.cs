using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Requests;
using Microsoft.AspNetCore.Identity;

namespace BankingSystemSharedDb.Db.Repositories;

public interface IUserRepository
{
    AccountEntity GetAccountByCardDetails(string cardNumber, int pin);
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
        var user = await Task.Run(() => _db.User.FirstOrDefault(u => u.PrivateNumber == privateNumber));

        return user;
    }

    public async Task<UserEntity?> FindWithId(int id)
    {
        var user = await Task.Run(() => _db.User.FirstOrDefault(u => u.Id == id));

        return user;
    }
    
    public async Task<UserEntity?> FindWithEmail(string email)
    {
        var user = await Task.Run(() => _db.User.FirstOrDefault(u => u.Email == email));
        
        return user;
    }

    public async Task Register(RegisterUserRequest request)
    {
        var user = new UserEntity
        {
            FirstName = request.Name,
            LastName = request.Surname,
            PrivateNumber = request.PrivateNumber,
            Email = request.Email,
            BirthDate = request.BirthDate,
            CreationDate = DateTime.Now
        };
        
        await _userManager.CreateAsync(user, request.Password);
        await _userManager.AddToRoleAsync(user, "user");
    }

    public async Task CreateCard(CardEntity cardEntity)
    {
        await _db.AddAsync(cardEntity);
        await _db.SaveChangesAsync();
    }

    public async Task<UserEntity?> GetUserWithEmail(string email)
    {
        var user = await Task.Run(() => _db.User.FirstOrDefault(u => u.Email == email));

        return user;
    }
    
    public async Task<UserEntity?> GetOperatorWithEmail(string email)
    {
        var operatorEntity = await Task.Run(() => _db.Users.FirstOrDefault(u => u.Email == email));

        return operatorEntity;
    }
    
    public AccountEntity GetAccountByCardDetails(string cardNumber, int pin)
    {
        var card = _db.Card.FirstOrDefault(c => c.CardNumber == cardNumber && c.Pin == pin);
        if (card == null)
        {
            throw new UnauthorizedAccessException("Invalid card number or PIN code");
        }

        var cardAccountConnection = _db.CardAccountConnection.FirstOrDefault(c => c.CardId == card.Id);
        if (cardAccountConnection == null)
        {
            throw new Exception("No account found for the card");
        }

        var account = _db.Account.FirstOrDefault(a => a.Iban == cardAccountConnection.Iban);
        if (account == null)
        {
            throw new Exception("No account found for the card");
        }

        return account;
    }
}