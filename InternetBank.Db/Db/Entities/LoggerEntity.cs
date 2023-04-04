using System.Collections;

namespace InternetBank.Db.Db.Entities;

public class LoggerEntity
{
    public int Id { get; set; }
    public string? ApiName { get; set; }
    public string? Exception { get; set; }
    public string? StackTrace { get; set; }
    public string? Data { get; set; }
    public DateTimeOffset ThrowTime { get; set; }
}