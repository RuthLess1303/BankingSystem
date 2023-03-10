namespace InternetBankCore.Db.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    public string PrivateNumber { get; set; }
    public string Iban { get; set; }
    public string CurrencyCode { get; set; }
    public decimal Amount { get; set; }
    public string Hash { get; set; }
    public DateTime CreationDate { get; set; }
}