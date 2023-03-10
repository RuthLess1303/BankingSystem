namespace InternetBankCore.Db.Entities;

public class CardAccountConnectionEntity
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public string Iban { get; set; }
    public string Hash { get; set; }
    public DateTime CreationDate { get; set; }
}