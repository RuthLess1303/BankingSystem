namespace InternetBank.Db.Db.Entities;

public class TransactionEntity
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public decimal GrossAmount { get; set; }
    public string SenderIban { get; set; }
    public string ReceiverIban { get; set; }
    public string CurrencyCode { get; set; }
    public decimal Fee { get; set; }
    public string Type { get; set; }
    public decimal Rate { get; set; }
    public DateTimeOffset TransactionTime { get; set; }
}