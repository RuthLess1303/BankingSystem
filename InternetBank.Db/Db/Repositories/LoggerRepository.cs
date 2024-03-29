using InternetBank.Db.Db.Entities;

namespace InternetBank.Db.Db.Repositories;

public interface ILoggerRepository
{
    Task AddLogInDb(Exception exception, string apiName);
    Task AddLog(string data, string apiName);
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
            ProjectName = apiName,
            Exception = exception.Message,
            StackTrace = exception.StackTrace,
            Data = exception.Data.ToString(),
            ThrowTime = DateTimeOffset.Now
        };

        await _db.Logger.AddAsync(loggerEntity);
        await _db.SaveChangesAsync();
    }

    public async Task AddLog(string data, string apiName)
    {
        var loggerEntity = new LoggerEntity
        {
            ProjectName = apiName,
            Data = data,
            ThrowTime = DateTimeOffset.Now
        };

        await _db.Logger.AddAsync(loggerEntity);
        await _db.SaveChangesAsync();
    }
}