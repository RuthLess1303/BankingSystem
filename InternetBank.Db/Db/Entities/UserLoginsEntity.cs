namespace InternetBank.Db.Db.Entities;

public class UserLoginsEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset LoginDate { get; set; }
}