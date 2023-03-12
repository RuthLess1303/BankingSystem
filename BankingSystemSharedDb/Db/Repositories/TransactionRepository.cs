using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Requests;

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
        var aggressor = await Task.Run(() => _db.Account.FirstOrDefault(a => a.Iban == request.AggressorIban));
        var receiver = await Task.Run(() => _db.Account.FirstOrDefault(a => a.Iban == request.ReceiverIban));

        aggressor.Amount -= request.Amount;
        receiver.Amount += request.Amount;
        
        await _db.SaveChangesAsync();
    }
    
    public async Task MakeTransactionWithFee(TransactionRequest request)
    {
        var aggressor = await Task.Run(() => _db.Account.FirstOrDefault(a => a.Iban == request.AggressorIban));
        var receiver = await Task.Run(() => _db.Account.FirstOrDefault(a => a.Iban == request.ReceiverIban));

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