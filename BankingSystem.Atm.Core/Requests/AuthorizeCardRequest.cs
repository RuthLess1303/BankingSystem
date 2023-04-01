namespace BankingSystem.Atm.Core.Requests;

public class AuthorizeCardRequest
{
    public string CardNumber { get; set; }
    public string PinCode { get; set; }
}