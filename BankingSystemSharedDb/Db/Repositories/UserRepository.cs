using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db.Repositories;

public interface IUserRepository
{
    Task CreateCard(CardEntity cardEntity);
    Task<UserEntity?> FindWithPrivateNumber(string privateNumber);
    Task<UserEntity?> FindWithId(Guid id);
    Task<UserEntity?> FindWithEmail(string email);
    Task Register(RegisterUserRequest request);
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
        var user = await _db.User.FirstOrDefaultAsync(u => u.PrivateNumber == privateNumber);

        return user;
    }

    public async Task<UserEntity?> FindWithId(Guid id)
    {
        var user = await _db.User.FirstOrDefaultAsync(u => u.Id == id);

        return user;
    }
    
    public async Task<UserEntity?> FindWithEmail(string email)
    {
        var user = await _db.User.FirstOrDefaultAsync(u => u.Email == email);
        
        return user;
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
    }
    

    public async Task<UserEntity?> GetUserWithEmail(string email)
    {
        var user = await _db.User.FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }
    
    public async Task<UserEntity?> GetOperatorWithEmail(string email)
    {
        var operatorEntity = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        return operatorEntity;
    }
    
}