namespace BankingSystemSharedDb.Db.Entities;

public class CardEntity
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public string Cvv { get; set; }
    public string Pin { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreationDate { get; set; }
}