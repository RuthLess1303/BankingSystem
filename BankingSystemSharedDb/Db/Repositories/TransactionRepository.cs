using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Requests;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db.Repositories;

public interface ITransactionRepository
{
    Task MakeTransaction(TransactionRequest request);
    Task MakeTransactionWithFee(TransactionRequest request);
    Task AddDataInDb(TransactionEntity entity);
}
public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task MakeTransaction(TransactionRequest request)
    {
        var aggressor = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.AggressorIban);
        if (aggressor == null)
        {
            throw new Exception($"Aggressor with {request.AggressorIban} does not exist");
        }
        var receiver = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.ReceiverIban);
        if (receiver == null)
        {
            throw new Exception($"Receiver with {request.ReceiverIban} does not exist");
        }
        try
        {
            var aggressor = await GetAccount(request.AggressorIban);
            var receiver = await GetAccount(request.ReceiverIban);

            aggressor.Balance -= request.Amount;
            receiver.Balance += request.Amount;
            
            await SaveChangesAsync();
            var transaction = new TransactionEntity
            {
                AggressorIban = request.AggressorIban,
                ReceiverIban = request.ReceiverIban,
                Amount = request.Amount,
                Type = "Internal",
                Fee = 0,
                TransactionTime = DateTime.Now
            };
            
            // aggressor.OutgoingTransactions.Add(transaction);
            // receiver.IncomingTransactions.Add(transaction);

            await SaveChangesAsync();
            await AddDataInDb(transaction);
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task MakeTransactionWithFee(TransactionRequest request)
    {
        var aggressor = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.AggressorIban);
        if (aggressor == null)
        {
            throw new Exception($"Aggressor with {request.AggressorIban} does not exist");
        }
        var receiver = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.ReceiverIban);
        if (receiver == null)
        {
            throw new Exception($"Receiver with {request.ReceiverIban} does not exist");
        }

        try
        {
            var aggressor = await GetAccount(request.AggressorIban);
            var receiver = await GetAccount(request.ReceiverIban);

            aggressor.Balance -= request.Amount * (decimal)1.01 + (decimal)0.5;
            receiver.Balance += request.Amount;

            await SaveChangesAsync();
            var transaction = new TransactionEntity
            {
                AggressorIban = request.AggressorIban,
                ReceiverIban = request.ReceiverIban,
                Amount = request.Amount,
            };
            
            // aggressor.OutgoingTransactions.Add(transaction);
            // receiver.IncomingTransactions.Add(transaction);

            await AddDataInDb(transaction);
            await dbTransaction.CommitAsync();
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task AddDataInDb(TransactionEntity transaction)
    {
        await _db.Transaction.AddAsync(transaction);
        await SaveChangesAsync();
    }
    
    private async Task<AccountEntity> GetAccount(string iban)
     {
         var account = await _db.Account.FindAsync(iban);
         if (account == null)
         {
             throw new ArgumentException("Account not found with the given IBAN.", nameof(iban));
         }
         return account;
     }
         
     private async Task SaveChangesAsync()
     {
         try
         {
             await _db.SaveChangesAsync();
         }
         catch (DbUpdateException ex)
         {
             throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
         }
     }
}