using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ICurrencyRepository
{
    Task<CurrencyEntity> FindCurrency(string currencyCode);
    Task<CurrencyEntity> FindCurrencyWithDate(string currencyCode, DateTimeOffset date);
}

public class CurrencyRepository : ICurrencyRepository
{
    private readonly AppDbContext _db;

    public CurrencyRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CurrencyEntity> FindCurrency(string currencyCode)
    {
        var currency = await _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefaultAsync(c => c.Code == currencyCode);
        if (currency == null)
        {
            throw new Exception("Could not find currency");
        }
       
        return currency;
    }

    public async Task<CurrencyEntity> FindCurrencyWithDate(string currencyCode, DateTimeOffset date)
    {
        var currencySameDate = await _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefaultAsync(c => c.Code.ToUpper() == currencyCode.ToUpper() && c.Date.DayOfYear == date.DayOfYear);

        if (currencySameDate != null)
        {
            return currencySameDate;
        }
        var currency = await _db.Currency
            .OrderByDescending(c => c.Date)
            .FirstOrDefaultAsync(c => c.Code == currencyCode);
        if (currency == null)
        {
            throw new Exception("Currency not found");
        }

        return currency;
    }
}