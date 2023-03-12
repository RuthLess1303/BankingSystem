namespace BankingSystemSharedDb.Db.Entities;

public class CardEntity
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public ushort Cvv { get; set; }
    public ushort Pin { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreationDate { get; set; }
}