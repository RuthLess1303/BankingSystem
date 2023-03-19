namespace AtmCore.Requests;

public class ChangePinRequest
{
    public string CardNumber { get; set; }
    public string PinCode { get; set; }
    public string NewPin { get; set; }
}