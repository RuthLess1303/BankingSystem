using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db.Repositories;

public interface IUserStatisticsRepository
{
    Task<long> TotalRegisteredUsersCurrentYear();
    Task<long> TotalRegisteredUsersForLastYear();
    Task<long> TotalRegisteredUsersForLast30Days();
}

public class UserStatisticsRepository : IUserStatisticsRepository
{
    private readonly AppDbContext _db;

    public UserStatisticsRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<long> TotalRegisteredUsersCurrentYear()
    {
        var users = await _db.User.Where(u => u.CreationDate.Year == DateTime.Now.Year).ToListAsync();

        return users.Count;
    }

    public async Task<long> TotalRegisteredUsersForLastYear()
    {
        var users = await _db.User.Where(u => u.CreationDate >= DateTime.Now.AddYears(-1)).ToListAsync();
        
        return users.Count;
    }
    
    public async Task<long> TotalRegisteredUsersForLast30Days()
    {
        var users = await _db.User.Where(u => u.CreationDate >= DateTime.Now.AddDays(-30)).ToListAsync();
        
        return users.Count;
    }
}