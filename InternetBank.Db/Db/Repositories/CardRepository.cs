using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ICardRepository
{
    Task<CardEntity> FindCardEntityByCardNumberAsync(string cardNumber);
    Task LinkWithAccount(string iban, Guid cardId);
    Task IsCardNumberInUse(string cardNumber);
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

    public async Task IsCardNumberInUse(string cardNumber)
    {
        var card = await _db.Card.FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

        if (card != null) throw new ArgumentException("Card Number already in use.");
    }

    public async Task LinkWithAccount(string iban, Guid cardId)
    {
        var account = await _db.Account.FirstOrDefaultAsync(a => a.Iban == iban);
        if (account == null)
        {
            throw new Exception($"Could not find account with specified IBAN: {iban}");
        }
        
        var cardAccountConnectionEntity = new CardAccountConnectionEntity
        {
            Iban = iban,
            CardId = cardId,
            CreationDate = DateTime.Now
        };

        await _db.AddAsync(cardAccountConnectionEntity);
        await _db.SaveChangesAsync();
    }
}