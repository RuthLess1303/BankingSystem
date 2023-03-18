using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

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
        if (currencyCode.ToUpper() == "GEL")
        {
            return null;
        }
        var currency = await _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefaultAsync(c => c.Code == currencyCode);
        if (currency == null)
        {
            throw new Exception("Could not find currency");
        }
       
        return currency;
    }
}