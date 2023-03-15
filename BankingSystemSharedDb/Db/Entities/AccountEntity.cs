namespace BankingSystemSharedDb.Db.Entities;

public class AccountEntity
{
    public Guid Id { get; set; }
    public string PrivateNumber { get; set; }
    public string Iban { get; set; }
    public string CurrencyCode { get; set; }
    public decimal Balance { get; set; }
    public string Hash { get; set; }
    public DateTime CreationDate { get; set; }
    public string UserEntityId { get; set; }
    public UserEntity user { get; set; }
    public ICollection<CardEntity> Cards { get; set; }
    public ICollection<TransactionEntity> IncomingTransactions { get; set; }
    public ICollection<TransactionEntity> OutgoingTransactions { get; set; }
}