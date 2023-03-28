namespace InternetBank.Db.Requests;

public class CardAuthorizationRequest
{
    public string CardNumber { get; set; }
    public string PinCode { get; set; }
}