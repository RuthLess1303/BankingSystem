namespace InternetBank.Db.Db.Entities;

public class CardAccountConnectionEntity
{
    public long Id { get; set; }
    public Guid CardId { get; set; }
    public string Iban { get; set; }
    public DateTime CreationDate { get; set; }
}