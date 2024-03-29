using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Reporting.Core.Repositories;

public interface ITransactionStatisticsRepository
{
    (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TransactionsLastMonths(int month);
    (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TotalTransactionsLastYear(int year);

    IQueryable<TransactionEntity> AtmTransactions();
    IQueryable<TransactionEntity> GelAllTransactions();
    Task<Dictionary<int, long>> TotalQuantitiesBasedOnDays();
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
            .Where(t => t.TransactionTime >= DateTimeOffset.Now.AddMonths(-month))
            .Where(t => t.Type.ToLower() == "outside");

        return (innerTransactions, outsideTransactions);
    }

    public (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TotalTransactionsLastYear(int year)
    {
        var innerTransactions = _db.Transaction
            .Where(t => t.TransactionTime >= DateTimeOffset.Now.AddYears(-year))
            .Where(t => t.Type.ToLower() == "inner");
        
        var outsideTransactions = _db.Transaction
            .Where(t => t.TransactionTime >= DateTimeOffset.Now.AddYears(-year))
            .Where(t => t.Type.ToLower() == "outside");

        return (innerTransactions, outsideTransactions);
    }

    public IQueryable<TransactionEntity> AtmTransactions()
    {
        var transactions = _db.Transaction.Where(t => t.Type.ToLower() == "atm");
    
        return transactions;
    }
    
    public async Task<Dictionary<int, long>> TotalQuantitiesBasedOnDays()
    {
        var transactionByDays = new Dictionary<int, long>();
        for (var i = 0; i <= 30; i++)
        {
            var transactions = await _db.Transaction.Where(t =>
                t.Type.ToLower() != "atm" && t.TransactionTime.DayOfYear == DateTimeOffset.Now.AddDays(-i).DayOfYear).ToListAsync();
            
            transactionByDays.Add(i + 1, transactions.Count);
        }
        
        return transactionByDays;
    }

    public IQueryable<TransactionEntity> GelAllTransactions()
    {
        var transactions = _db.Transaction
            .Where(t => t.Type.ToLower() != "atm");

        return transactions;
    }
}