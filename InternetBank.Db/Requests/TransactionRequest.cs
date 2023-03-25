namespace InternetBank.Db.Requests;

public class TransactionRequest
{
    public string SenderIban { get; set; }
    public string ReceiverIban { get; set; }
    public decimal Amount { get; set; }
}