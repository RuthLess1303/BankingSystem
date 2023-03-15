using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db.Repositories;

public interface IAccountRepository
{
    Task<string> GetAccountCurrencyCode(string iban);
    Task<AccountEntity?> GetAccountWithIban(string iban);
    Task<decimal> GetAccountMoney(string iban);
    Task<List<TransactionEntity>> GetAggressorTransactions(string iban);
    Task<List<TransactionEntity>> GetReceiverTransactions(string iban);
    Task<TransactionEntity?> HasTransaction(string iban);
    Task<CardEntity?> GetCardWithIban(string iban);
    Task Create(AccountEntity accountEntity);
    Task<UserEntity> GetUserByIban(string iban);
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
        var account = await  _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        return account?.CurrencyCode;
    }

    public async Task<AccountEntity?> GetAccountWithIban(string iban)
    {
        return await  _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
    }

    public async Task<decimal> GetAccountMoney(string iban)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        return account?.Balance ?? 0;
    }

    public async Task<List<TransactionEntity>> GetAggressorTransactions(string iban)
    {
        return await _db.Transaction.Where(t => t.AggressorIban == iban).ToListAsync();
    }
    
    public async Task<List<TransactionEntity>> GetReceiverTransactions(string iban)
    {
        return await _db.Transaction.Where(t => t.ReceiverIban == iban).ToListAsync();
    }

    public async Task<TransactionEntity?> HasTransaction(string iban)
    {
        return await _db.Transaction.FirstOrDefaultAsync(t => t.ReceiverIban == iban);
    }

    public async Task<CardEntity?> GetCardWithIban(string iban)
    {
        var cardAccountConnection = await _db.CardAccountConnection.FirstOrDefaultAsync(c => c.Iban == iban);
        var card = await _db.Card.FirstOrDefaultAsync(c => c.Id == cardAccountConnection.CardId);
        return card;
    }

    public async Task Create(AccountEntity accountEntity)
    {
        try
        {
            await _db.AddAsync(accountEntity);
            await _db.SaveChangesAsync();

            // Add the account to the user's collection
            var user = await GetUserByIban(accountEntity.Iban);
            user.Accounts.Add(accountEntity);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var message = $"An error occurred while adding account with IBAN {accountEntity.Iban}.";
            throw new Exception(message, ex);
        }
    }
    
    public async Task<UserEntity> GetUserByIban(string iban)
    {
        var user = await _db.Users.Include(u => u.Accounts)
            .Where(u => u.Accounts.Any(a => a.Iban == iban))
            .FirstOrDefaultAsync();
        return user;
    }
}