namespace BankingSystemSharedDb.Db.Models;

public class CardModel
{
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public ushort Cvv { get; set; }
    public DateTime ExpirationDate { get; set; }
}