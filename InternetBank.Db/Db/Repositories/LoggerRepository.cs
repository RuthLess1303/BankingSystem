using InternetBank.Db.Db.Entities;

namespace InternetBank.Db.Db.Repositories;

public interface ILoggerRepository
{
    Task AddLogInDb(Exception exception, string apiName);
}

public class LoggerRepository : ILoggerRepository
{
    private readonly AppDbContext _db;

    public LoggerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddLogInDb(Exception exception, string apiName)
    {
        var loggerEntity = new LoggerEntity()
        {
            ApiName = apiName,
            Exception = exception.Message,
            StackTrace = exception.StackTrace,
            Data = exception.Data.ToString(),
            ThrowTime = DateTimeOffset.Now
        };

        await _db.Logger.AddAsync(loggerEntity);
        await _db.SaveChangesAsync();
    }
}