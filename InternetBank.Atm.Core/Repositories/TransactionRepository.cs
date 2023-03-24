using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Atm.Core.Repositories;

public interface ITransactionRepository
{
    Task<decimal> GetWithdrawalsInLast24HoursAsync(string iban);
    Task AddTransactionInDb(TransactionEntity entity);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<decimal> GetWithdrawalsInLast24HoursAsync(string iban)
    {
        var withdrawalsInLast24Hours = await _db.Transaction
            .Where(t => t.ReceiverIban == iban && t.Type == "ATM" && t.TransactionTime >= DateTime.UtcNow.AddDays(-1))
            .SumAsync(t => t.Amount);
        return withdrawalsInLast24Hours;
    }

    public async Task AddTransactionInDb(TransactionEntity entity)
    {
        await _db.Transaction.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
}