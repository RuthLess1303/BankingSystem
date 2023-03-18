namespace AtmCore.Requests;

public class WithdrawalRequest
{
    public string CardNumber { get; set; }
    public string PinCode { get; set; }
    public decimal Amount { get; set; }
}