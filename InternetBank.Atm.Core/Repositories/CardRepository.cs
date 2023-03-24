using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Atm.Core.Repositories;

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
        return card;
    }
}