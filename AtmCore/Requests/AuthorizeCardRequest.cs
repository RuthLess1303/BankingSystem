namespace AtmCore.Requests;

public class AuthorizeCardRequest
{
    public string CardNumber { get; set; }
    public string PinCode { get; set; }
}