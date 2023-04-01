namespace InternetBank.Db.Db.Entities;

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
    public DateTimeOffset Date { get; set; }
    public DateTimeOffset ValidFromDate { get; set; }
}