namespace InternetBank.Db.Db.Entities;

public class LoginLoggerEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset LoginDate { get; set; }
}