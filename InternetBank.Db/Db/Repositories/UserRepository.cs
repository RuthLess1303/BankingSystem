using InternetBank.Db.Db.Entities;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface IUserRepository
{
    Task CreateCard(CardEntity cardEntity);
    Task<UserEntity?> FindWithPrivateNumber(string privateNumber);
    Task<UserEntity?> FindWithId(int id);
    Task<UserEntity?> FindWithEmail(string email);
    Task Register(RegisterUserRequest request);
    Task<UserEntity?> GetUserWithEmail(string email);
    Task<UserEntity?> GetOperatorWithEmail(string email);
    Task<UserEntity> GetUserWithIban(string iban);
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
        return  await _db.User.FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async Task<UserEntity?> FindWithEmail(string email)
    {
        return  await _db.User.FirstOrDefaultAsync(u => u.Email == email);
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
        await _db.Card.AddAsync(cardEntity);
        await _db.SaveChangesAsync();
    }
    
    public async Task<UserEntity?> GetUserWithEmail(string email)
    {
        return await _db.User.FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<UserEntity?> GetOperatorWithEmail(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<UserEntity> GetUserWithIban(string iban)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        if (account == null)
        {
            throw new Exception("Account with provided Iban does not exist");
        }
        var user = await _db.User.FirstOrDefaultAsync(u => u.PrivateNumber == account.PrivateNumber);
        if (user == null)
        {
            throw new Exception("User with provided Iban does not exist");
        }

        return user;
    }
}