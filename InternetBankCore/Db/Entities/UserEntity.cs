namespace InternetBankCore.Db.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PrivateNumber { get; set; }
    public string Email { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreationDate { get; set; }
}