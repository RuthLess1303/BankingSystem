namespace BankingSystemSharedDb.Requests;

public class CreateAccountRequest
{
    public string PrivateNumber { get; set; }
    public string Iban { get; set; }
    public string CurrencyCode { get; set; }
    public decimal Amount { get; set; }
}