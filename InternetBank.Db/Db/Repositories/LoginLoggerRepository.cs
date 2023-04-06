using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ILoginLoggerRepository
{
    Task AddLoggedInUser(int userId);
    Task<LoginLoggerEntity> GetLoggedUser();
}

public class LoginLoggerRepository : ILoginLoggerRepository
{
    private readonly AppDbContext _db;

    public LoginLoggerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<LoginLoggerEntity> GetLoggedUser()
    {
        var user = await _db.LoginLogger.OrderByDescending(l => l.LoginDate).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new Exception("There is not any logged user");
        }

        return user;
    }

    public async Task AddLoggedInUser(int userId)
    {
        var loginUser = CreateLoginUserEntity(userId);
        await _db.LoginLogger.AddAsync(loginUser);
        await _db.SaveChangesAsync();
    }

    private LoginLoggerEntity CreateLoginUserEntity(int userId)
    {
        var loginUser = new LoginLoggerEntity
        {
            UserId = userId,
            LoginDate = DateTimeOffset.Now
        };

        return loginUser;
    }
}