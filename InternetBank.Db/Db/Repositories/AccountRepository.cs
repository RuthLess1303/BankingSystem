using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface IAccountRepository
{
    Task<string?> GetAccountCurrencyCode(string iban);
    Task<AccountEntity?> GetAccountWithIban(string iban);
    Task<decimal?> GetAccountMoney(string iban);
    Task<List<TransactionEntity>> GetAggressorTransactions(string iban);
    Task<List<TransactionEntity>> GetReceiverTransactions(string iban);
    Task<TransactionEntity?> HasTransaction(string iban);
    Task Create(AccountEntity accountEntity);
}

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string?> GetAccountCurrencyCode(string iban)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        var currencyCode = account.CurrencyCode;

        return currencyCode;
    }

    public async Task<AccountEntity?> GetAccountWithIban(string iban)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);

        return account;
    }

    public async Task<decimal?> GetAccountMoney(string iban)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        var amount = account.Balance;
        
        return amount;
    }

    public async Task<List<TransactionEntity>> GetAggressorTransactions(string iban)
    {
        var aggressorTransactions = await _db.Transaction.Where(t => t.SenderIban == iban).ToListAsync();

        return aggressorTransactions;
    }
    
    public async Task<List<TransactionEntity>> GetReceiverTransactions(string iban)
    {
        var receiverTransactions = await _db.Transaction.Where(t => t.ReceiverIban == iban).ToListAsync();

        return receiverTransactions;
    }

    public async Task<TransactionEntity?> HasTransaction(string iban)
    {
        var transaction = await _db.Transaction.FirstOrDefaultAsync(t => t.ReceiverIban == iban);

        return transaction;
    }

    public async Task Create(AccountEntity accountEntity)
    {
        await _db.AddAsync(accountEntity);
        await _db.SaveChangesAsync();
    }
}