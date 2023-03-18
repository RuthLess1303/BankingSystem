﻿using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace AtmCore.Repositories;

public interface IAccountRepository
{
    Task<AccountEntity> GetAccountByCardDetails(string cardNumber, string pin);
    Task<decimal> GetAccountMoney(string iban);
}

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AccountEntity> GetAccountByCardDetails(string cardNumber, string pin)
    {
        var card = await _db.Card.FirstOrDefaultAsync(c => c.CardNumber == cardNumber && c.Pin == pin);
        if (card == null) throw new UnauthorizedAccessException("Invalid card number or PIN code");

        var cardAccountConnection = await _db.CardAccountConnection.FirstOrDefaultAsync(c => c.CardId == card.Id);
        if (cardAccountConnection == null) throw new Exception("No account found for the card");

        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == cardAccountConnection.Iban);
        if (account == null) throw new Exception("No account found for the card");

        return account;
    }

    public async Task<decimal> GetAccountMoney(string iban)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        if (account == null) throw new Exception("No account found for the given IBAN");

        return account.Balance;
    }
}