using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface IAccountRepository
{
    Task<string> GetAccountCurrencyCode(string iban);
    Task<AccountEntity?> GetAccountWithIban(string iban);
    Task<decimal> GetBalance(string iban);
    Task<List<TransactionEntity>> GetSenderTransactions(string iban);
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

    public async Task<string> GetAccountCurrencyCode(string iban)
    {
        var account = await GetAccountByIban(iban);
        return account.CurrencyCode;
    }

    public async Task<AccountEntity?> GetAccountWithIban(string iban)
    {
        var account = await GetAccountByIban(iban);
        return account;
    }

    public async Task<decimal> GetBalance(string iban)
    {
        var account = await GetAccountByIban(iban);
        
        return account.Balance;
    }

    public async Task<List<TransactionEntity>> GetSenderTransactions(string iban)
    {
        var senderTransactions = await _db.Transaction.Where(t => t.SenderIban == iban).ToListAsync();
        return senderTransactions;
    }
    
    public async Task<List<TransactionEntity>> GetReceiverTransactions(string iban)
    {
        var receiverTransactions = await _db.Transaction.Where(t => t.ReceiverIban == iban).ToListAsync();
        return receiverTransactions;
    }

    public async Task<TransactionEntity?> HasTransaction(string iban)
    {
        var transactionReceiver = await _db.Transaction.FirstOrDefaultAsync(t => t.ReceiverIban == iban);
        if (transactionReceiver != null)
        {
            return transactionReceiver;
        }
        var transactionSender = await _db.Transaction.FirstOrDefaultAsync(t => t.SenderIban == iban);
        return transactionSender;
    }

    public async Task Create(AccountEntity accountEntity)
    {
        if (accountEntity == null)
            throw new ArgumentNullException(nameof(accountEntity));
        
        await _db.Account.AddAsync(accountEntity);
        await _db.SaveChangesAsync();
    }
    
    private async Task<AccountEntity> GetAccountByIban(string iban)
    {
        if (string.IsNullOrEmpty(iban))
            throw new Exception("The IBAN cannot be null or empty");

        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);

        return account;
    }
}