using BankingSystemSharedDb.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemSharedDb.Db.Repositories;

public interface ICardRepository
{
    Task<CardEntity> FindCardEntityByCardNumberAsync(string cardNumber);
}

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _db;

    public CardRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<CardEntity> FindCardEntityByCardNumberAsync(string cardNumber)
    {
        var card = await _db.Card.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

        if (card == null) throw new ArgumentException("Card entity not found with the given card number.");

        return card;
    }
}