using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace ReportingCore.Repositories;

public interface ITransactionStatisticsRepository
{
    (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TransactionsLastMonths(int month);
    (IQueryable<TransactionEntity>?, IQueryable<TransactionEntity>?) TotalTransactionsLastYear(int year);

    IQueryable<TransactionEntity> AtmTransactions();
    // Task<(long, long, long)> TotalTransactionsLast1Month();
    // Task<(long, long, long)> TotalTransactionsLast6Month();
    // Task<(long, long, long)> TotalTransactionsLast1Year();
    // Task<(decimal, decimal, decimal)> TotalTransactionIncomeLast1Month();
    // Task<(decimal, decimal, decimal)> TotalTransactionIncomeLast6Month();
    // Task<(decimal, decimal, decimal)> TotalTransactionIncomeLast1Year();
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
    // public async Task<(long, long, long)> TotalTransactionsLast1Month()
    // {
    //     var transactions = _db.Transaction.Where(t => t.TransactionTime >= DateTime.Now.AddMonths(-1));
    //     var innerTransactions = await transactions.Where(t => t.Type.ToLower() == "inner").ToListAsync();
    //     var outsideTransactions = await transactions.Where(t => t.Type.ToLower() == "outside").ToListAsync();
    //
    //     return (innerTransactions.Count, outsideTransactions.Count, innerTransactions.Count + outsideTransactions.Count);
    // }
    //
    // public async Task<(long, long, long)> TotalTransactionsLast6Month()
    // {
    //     var transactions = _db.Transaction.Where(t => t.TransactionTime >= DateTime.Now.AddMonths(-6));
    //     var innerTransactions = await transactions.Where(t => t.Type.ToLower() == "inner").ToListAsync();
    //     var outsideTransactions = await transactions.Where(t => t.Type.ToLower() == "outside").ToListAsync();
    //
    //     return (innerTransactions.Count, outsideTransactions.Count, innerTransactions.Count + outsideTransactions.Count);
    // }
    //
    // public async Task<(long, long, long)> TotalTransactionsLast1Year()
    // {
    //     var transactions = _db.Transaction.Where(t => t.TransactionTime >= DateTime.Now.AddYears(-1));
    //     var innerTransactions = await transactions.Where(t => t.Type.ToLower() == "inner").ToListAsync();
    //     var outsideTransactions = await transactions.Where(t => t.Type.ToLower() == "outside").ToListAsync();
    //
    //     return (innerTransactions.Count, outsideTransactions.Count, innerTransactions.Count + outsideTransactions.Count);

    // public async Task<(decimal, decimal, decimal)> TotalTransactionIncomeLast1Month()
    // {
    //     var transactions = _db.Transaction.Where(t => t.TransactionTime >= DateTime.Now.AddMonths(-1));
    //     var innerIncome = await transactions.Where(t => t.Type.ToLower() == "inner").SumAsync(s => s.Amount);
    //     var outsideIncome = await transactions.Where(t => t.Type.ToLower() == "outside").SumAsync(s => s.Amount);
    //
    //     return (innerIncome, outsideIncome, innerIncome + outsideIncome);
    // }
    //
    // public async Task<(decimal, decimal, decimal)> TotalTransactionIncomeLast6Month()
    // {
    //     var transactions = _db.Transaction.Where(t => t.TransactionTime >= DateTime.Now.AddMonths(-6));
    //     var innerIncome = await transactions.Where(t => t.Type.ToLower() == "inner").SumAsync(s => s.Amount);
    //     var outsideIncome = await transactions.Where(t => t.Type.ToLower() == "outside").SumAsync(s => s.Amount);
    //
    //     return (innerIncome, outsideIncome, innerIncome + outsideIncome);
    // }
    //
    // public async Task<(decimal, decimal, decimal)> TotalTransactionIncomeLast1Year()
    // {
    //     var transactions = _db.Transaction.Where(t => t.TransactionTime >= DateTime.Now.AddYears(-1));
    //     var innerIncome = await transactions.Where(t => t.Type.ToLower() == "inner").SumAsync(s => s.Amount);
    //     var outsideIncome = await transactions.Where(t => t.Type.ToLower() == "outside").SumAsync(s => s.Amount);
    //
    //     return (innerIncome, outsideIncome, innerIncome + outsideIncome);
    // }

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