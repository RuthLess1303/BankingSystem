namespace BankingSystemSharedDb.Requests;

public class CardAuthorizationRequest
{
    public string CardNumber { get; set; }
    public int PinCode { get; set; }
}