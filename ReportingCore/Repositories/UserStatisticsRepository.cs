using BankingSystemSharedDb.Db;
using Microsoft.EntityFrameworkCore;

namespace ReportingCore.Repositories;

public interface IUserStatisticsRepository
{
    long TotalRegisteredUsersCurrentYear();
    long TotalRegisteredUsersForLastYear();
    long TotalRegisteredUsersForLast30Days();
}

public class UserStatisticsRepository : IUserStatisticsRepository
{
    private readonly AppDbContext _db;

    public UserStatisticsRepository(AppDbContext db)
    {
        _db = db;
    }

    public long TotalRegisteredUsersCurrentYear()
    {
        var users = _db.User
            .Where(u => u.CreationDate.Year == DateTime.Now.Year);
        
        return users.Count();
    }

    public long TotalRegisteredUsersForLastYear()
    {
        var users = _db.User
            .Where(u => u.CreationDate >= DateTime.Now.AddYears(-1));
        
        return users.Count();
    }
    
    public long TotalRegisteredUsersForLast30Days()
    {
        var users = _db.User
            .Where(u => u.CreationDate >= DateTime.Now.AddDays(-30));
        
        return users.Count();
    }
}