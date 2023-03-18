namespace BankingSystemSharedDb.Requests;

public class CreateCardRequest
{
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public string Cvv { get; set; }
    public string Pin { get; set; }
    public string Iban { get; set; }
    public DateTime ExpirationDate { get; set; }
}