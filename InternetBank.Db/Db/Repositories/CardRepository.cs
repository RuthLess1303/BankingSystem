using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ICardRepository
{
    Task LinkWithAccount(string iban, Guid cardId);
    Task CardNumberUsage(string cardNumber);
    Task<CardEntity?> GetCardWithIban(string iban);
}

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _db;

    public CardRepository(AppDbContext db)
    {
        _db = db;
    }
    
    public async Task CardNumberUsage(string cardNumber)
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
    
    public async Task<CardEntity?> GetCardWithIban(string iban)
    {
        var cardAccountConnection = await _db.CardAccountConnection.FirstOrDefaultAsync(c => c.Iban == iban);
        if (cardAccountConnection == null)
        {
            throw new Exception($"Account with IBAN {iban} not found.");
        }
        var card = await _db.Card.FirstOrDefaultAsync(c => c.Id == cardAccountConnection.CardId);

        return card;
    }
}