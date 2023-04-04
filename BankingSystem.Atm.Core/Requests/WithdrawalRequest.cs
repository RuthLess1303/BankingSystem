namespace BankingSystem.Atm.Core.Requests;

public class WithdrawalRequest
{
    public string CardNumber { get; set; }
    public string PinCode { get; set; }
    public int Amount { get; set; }
}