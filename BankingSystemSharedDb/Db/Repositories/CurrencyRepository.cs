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
        return await _db.Currency.FirstOrDefaultAsync(c => c.Code == currencyCode);
    }
}