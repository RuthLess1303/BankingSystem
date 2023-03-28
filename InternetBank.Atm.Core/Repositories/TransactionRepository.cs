﻿using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Atm.Core.Repositories;

public interface ITransactionRepository
{
    Task<decimal> GetWithdrawalAmountInLast24HoursAsync(string iban);
    Task AddTransactionInDb(TransactionEntity entity);
}

public class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;
    private const string withdrawalType = "ATM";

    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task<decimal> GetWithdrawalAmountInLast24HoursAsync(string iban)
    {
        // Find the account entity for the specified IBAN
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        if (account == null)
        {
            throw new ArgumentException($"Account with IBAN {iban} does not exist");
        }

        // Find the user associated with the account using the PrivateNumber property
        var user = await _db.Users.FirstOrDefaultAsync(u => u.PrivateNumber == account.PrivateNumber);
        if (user == null)
        {
            throw new ArgumentException($"User with account IBAN {iban} does not exist");
        }

        // Find all account entities for the user using the PrivateNumber property
        var userAccounts = await _db.Account.Where(a => a.PrivateNumber == user.PrivateNumber).ToListAsync();

        //Retrieve the withdrawals for the user's accounts in the last 24 hours
        var withdrawalsInLast24Hours =  _db.Transaction.AsEnumerable()
            .Where(t => userAccounts.Any(a => a.Iban == t.ReceiverIban) && t.Type == withdrawalType && t.TransactionTime >=  DateTimeOffset.UtcNow.AddDays(-1))
            .Sum(t => t.Amount);
        
        // var withdrawalsInLast24Hours = await _db.Account
        //     .Join(
        //         _db.Transaction,
        //         a => a.Iban,
        //         transactionAccounts => transactionAccounts.ReceiverIban,
        //         (a, transactionAccounts) => new { Account = a, Transaction = transactionAccounts }
        //     )
        //     .Where(joinedEntities  => joinedEntities .Account.Iban == iban && joinedEntities .Transaction.Type == WithdrawalType && joinedEntities .Transaction.TransactionTime >= DateTimeOffset.UtcNow.AddDays(-1))
        //     .SumAsync(joinedEntities => joinedEntities.Transaction.Amount);
        
        
        return withdrawalsInLast24Hours;
    }


    public async Task AddTransactionInDb(TransactionEntity entity)
    {
        await _db.Transaction.AddAsync(entity);
        await _db.SaveChangesAsync();
    }
}