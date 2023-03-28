namespace InternetBank.Db.Db.Models;

public class CardModel
{
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public string Cvv { get; set; }
    public DateTime ExpirationDate { get; set; }
}