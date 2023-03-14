using Microsoft.AspNetCore.Identity;

namespace BankingSystemSharedDb.Db.Entities;

public class UserEntity : IdentityUser<int>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PrivateNumber { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreationDate { get; set; }
}