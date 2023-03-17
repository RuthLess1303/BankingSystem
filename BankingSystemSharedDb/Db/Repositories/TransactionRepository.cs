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

        aggressor.Amount -= request.Amount;
        receiver.Amount += request.Amount;
        
        await _db.SaveChangesAsync();
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

        aggressor.Amount -= request.Amount * (decimal)1.01 + (decimal)0.5;
        receiver.Amount += request.Amount;
        
        await _db.SaveChangesAsync();
    }

    public async Task AddDataInDb(TransactionEntity entity)
    {
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
}