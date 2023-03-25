using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Reporting.Core.Repositories;

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
    
    public async Task<Dictionary<int, long>> TotalQuantitiesBasedOnDays()
    {
        var transactionByDays = new Dictionary<int, long>();
        for (int i = 1; i <= 31; i++)
        {
            var transactions = await _db.Transaction.Where(t =>
                t.Type.ToLower() == "atm" && t.TransactionTime == DateTime.Now.AddDays(-i)).ToListAsync();
            
            transactionByDays.Add(i, transactions.Count);
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