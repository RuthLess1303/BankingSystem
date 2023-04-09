using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystem.Atm.Core.Repositories;

public interface ITransactionRepository
{
    Task<decimal> GetWithdrawalAmountInLast24HoursAsync(string iban);
    Task AddTransactionInDb(TransactionEntity entity);
}

public class TransactionRepository : ITransactionRepository
{
    private const string WithdrawalType = "ATM";
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<decimal> GetWithdrawalAmountInLast24HoursAsync(string iban)
    {
        // Find the account entity for the specified IBAN
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        if (account == null) throw new ArgumentException($"Account with IBAN {iban} does not exist");

        // Find the user associated with the account using the PrivateNumber property
        var user = await _db.Users.FirstOrDefaultAsync(u => u.PrivateNumber == account.PrivateNumber);
        if (user == null) throw new ArgumentException($"User with account IBAN {iban} does not exist");

        // Find all account entities for the user using the PrivateNumber property
        var userAccounts = await _db.Account.Where(a => a.PrivateNumber == user.PrivateNumber).ToListAsync();

        //Retrieve the withdrawals for the user's accounts in the last 24 hours
        var withdrawalsInLast24Hours = _db.Transaction.AsEnumerable()
            .Where(t => userAccounts.Any
                            (a => a.Iban == t.ReceiverIban)
                        && t.Type == WithdrawalType
                        && t.TransactionTime >= DateTimeOffset.UtcNow.AddDays(-1))
            .Sum(t => t.Amount);

        return withdrawalsInLast24Hours;
    }

    public async Task AddTransactionInDb(TransactionEntity entity)
    {
        await _db.Transaction.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
}