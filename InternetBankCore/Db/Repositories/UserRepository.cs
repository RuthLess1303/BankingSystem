using InternetBankCore.Db.Entities;
using InternetBankCore.Requests;
using MobileBank.Requests;

namespace InternetBankCore.Db.Repositories;

public interface IUserRepository
{
    Task<UserEntity?> FindWithPrivateNumber(string privateNumber);
    Task<UserEntity?> FindWithId(Guid id);
    Task<UserEntity?> FindWithEmail(string email);
    Task Register(RegisterUserRequest request);
    Task CreateCard(CardEntity cardEntity);
    Task<UserEntity?> GetUserWithEmail(string email);
    Task<OperatorEntity?> GetOperatorWithEmail(string email);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UserEntity?> FindWithPrivateNumber(string privateNumber)
    {
        var user = await Task.Run(() => _db.User.FirstOrDefault(u => u.PrivateNumber == privateNumber));

        return user;
    }

    public async Task<UserEntity?> FindWithId(Guid id)
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
            Id = Guid.NewGuid(),
            Password = request.Password,
            Name = request.Name,
            Surname = request.Surname,
            PrivateNumber = request.PrivateNumber,
            Email = request.Email,
            BirthDate = request.BirthDate,
            CreationDate = DateTime.Now
        };

        await _db.AddAsync(user);
        await _db.SaveChangesAsync();
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
    
    public async Task<OperatorEntity?> GetOperatorWithEmail(string email)
    {
        var operatorEntity = await Task.Run(() => _db.Operator.FirstOrDefault(u => u.Email == email));

        return operatorEntity;
    }
}