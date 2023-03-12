namespace BankingSystemSharedDb.Requests;

public class CreateCardRequest
{
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public ushort Cvv { get; set; }
    public ushort Pin { get; set; }
    public DateTime ExpirationDate { get; set; }
}