using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace ReportingCore.Repositories;

public interface ITransactionStatisticsRepository
{
    (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TransactionsLastMonths(int month);
    (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TotalTransactionsLastYear(int year);

    IQueryable<TransactionEntity> AtmTransactions();
    IQueryable<TransactionEntity> GelAllTransactions();
    Task<List<DailyTransaction>> TotalQuantitiesBasedOnDays();
}

public class TransactionStatisticsRepository : ITransactionStatisticsRepository
{
    private readonly AppDbContext _db;

    public TransactionStatisticsRepository(AppDbContext db)
    {
        _db = db;
    }

    public (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TransactionsLastMonths(int month)
    {
        var innerTransactions = _db.Transaction
            .Where(t => t.TransactionTime >= DateTime.Now.AddMonths(-month))
            .Where(t => t.Type.ToLower() == "inner");

        var outsideTransactions = _db.Transaction
            .Where(t => t.TransactionTime >= DateTime.Now.AddMonths(-month))
            .Where(t => t.Type.ToLower() == "outside");

        return (innerTransactions, outsideTransactions);
    }

    public (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TotalTransactionsLastYear(int year)
    {
        var innerTransactions = _db.Transaction
            .Where(t => t.TransactionTime >= DateTime.Now.AddYears(-year))
            .Where(t => t.Type.ToLower() == "inner");

        var outsideTransactions = _db.Transaction
            .Where(t => t.TransactionTime >= DateTime.Now.AddYears(-year))
            .Where(t => t.Type.ToLower() == "outside");

        return (innerTransactions, outsideTransactions);
    }

    public IQueryable<TransactionEntity> AtmTransactions()
    {
        var transactions = _db.Transaction.Where(t => t.Type.ToLower() == "atm");

        return transactions;
    }

    public async Task<List<DailyTransaction>> TotalQuantitiesBasedOnDays()
    {
        var transactions = await _db.Transaction
            .Where(t => t.Type.ToLower() == "atm" && t.TransactionTime >= DateTime.Now.AddMonths(-1))
            .GroupBy(t => t.TransactionTime.Day)
            .Select(g => new DailyTransaction { Day = g.Key, Count = g.Count()})
            .ToListAsync();
        return transactions;
    }

    public IQueryable<TransactionEntity> GelAllTransactions()
    {
        var transactions = _db.Transaction
            .Where(t => t.Type.ToLower() != "atm");

        return transactions;
    }
}

public class DailyTransaction
{
    public int Day { get; set; }
    public int Count { get; set; }
}