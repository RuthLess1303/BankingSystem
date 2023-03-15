namespace BankingSystemSharedDb.Db.Entities;

public class CardAccountConnectionEntity
{
    public long Id { get; set; }
    public Guid CardId { get; set; }
    public string Iban { get; set; }
    public string Hash { get; set; }
    public DateTime CreationDate { get; set; }

    public CardEntity Card { get; set; }
    public AccountEntity Account { get; set; }
}