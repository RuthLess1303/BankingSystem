using InternetBank.Db.Db.Entities;
using InternetBank.Db.Requests;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ITransactionRepository
{
    Task MakeTransaction(TransactionRequest request, decimal convertedAmount);
    Task MakeTransactionWithFee(TransactionRequest request, decimal convertedAmount);
    Task AddDataInDb(TransactionEntity entity);
}
public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;
    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task MakeTransaction(TransactionRequest request, decimal convertedAmount)
    {
        var receiver = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.ReceiverIban);
        if (receiver == null)
        {
            throw new Exception($"Could not find account with iban: {request.ReceiverIban}");
        }
        var sender = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.SenderIban);
        if (sender == null)
        {
            throw new Exception($"Could not find account with iban: {request.SenderIban}");
        }

        sender.Balance -= request.Amount;
        receiver.Balance += convertedAmount;
        
        await _db.SaveChangesAsync();
    }
    
    public async Task MakeTransactionWithFee(TransactionRequest request, decimal convertedAmount)
    {
        var receiver = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.ReceiverIban);
        if (receiver == null)
        {
            throw new Exception($"Could not find account with iban: {request.ReceiverIban}");
        }
        var sender = await _db.Account.FirstOrDefaultAsync(a => a.Iban == request.SenderIban);
        if (sender == null)
        {
            throw new Exception($"Could not find account with iban: {request.SenderIban}");
        }

        sender.Balance -= request.Amount * (decimal)1.01 + (decimal)0.5;
        receiver.Balance += convertedAmount;
        
        await _db.SaveChangesAsync();
    }

    public async Task AddDataInDb(TransactionEntity entity)
    {
        await _db.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
}