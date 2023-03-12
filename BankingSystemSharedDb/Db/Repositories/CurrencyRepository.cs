using BankingSystemSharedDb.Db.Entities;
using InternetBankCore.Db;

namespace BankingSystemSharedDb.Db.Repositories;

public interface ICurrencyRepository
{
    Task<CurrencyEntity?> FindCurrency(string currencyCode);
}

public class CurrencyRepository : ICurrencyRepository
{
    private readonly AppDbContext _db;

    public CurrencyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CurrencyEntity?> FindCurrency(string currencyCode)
    {
        var currency = await Task.Run(() => _db.Currency.FirstOrDefault(c => c.Code == currencyCode));

        return currency;
    }
}