namespace BankingSystemSharedDb.Requests;

public class RegisterUserRequest
{
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string PrivateNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime BirthDate { get; set; }
}