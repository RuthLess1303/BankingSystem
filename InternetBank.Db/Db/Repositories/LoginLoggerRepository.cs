using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ILoginLoggerRepository
{
    Task AddLoggedInUser(int userId);
    Task<UserLoginsEntity> GetLoggedUser();
}

public class LoginLoggerRepository : ILoginLoggerRepository
{
    private readonly AppDbContext _db;

    public LoginLoggerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<UserLoginsEntity> GetLoggedUser()
    {
        var user = await _db.UserLogins.OrderByDescending(l => l.LoginDate).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new Exception("There is not any logged user");
        }

        return user;
    }

    public async Task AddLoggedInUser(int userId)
    {
        var loginUser = CreateLoginUserEntity(userId);
        await _db.UserLogins.AddAsync(loginUser);
        await _db.SaveChangesAsync();
    }

    private UserLoginsEntity CreateLoginUserEntity(int userId)
    {
        var loginUser = new UserLoginsEntity
        {
            UserId = userId,
            LoginDate = DateTimeOffset.Now
        };

        return loginUser;
    }
}