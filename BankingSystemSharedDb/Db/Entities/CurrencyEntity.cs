namespace BankingSystemSharedDb.Db.Entities;

public class CurrencyEntity
{
    public long Id { get; set; }
    public string Code { get; set; }
    public int Quantity { get; set; }
    public decimal RateFormatted { get; set; }
    public decimal DiffFormatted { get; set; }
    public decimal Rate { get; set; }
    public string Name { get; set; }
    public decimal Diff { get; set; }
    public DateTime Date { get; set; }
    public DateTime ValidFromDate { get; set; }
}