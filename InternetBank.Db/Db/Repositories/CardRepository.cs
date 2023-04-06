using InternetBank.Db.Db.Entities;
using Microsoft.EntityFrameworkCore;

namespace InternetBank.Db.Db.Repositories;

public interface ICardRepository
{
    Task LinkWithAccount(string iban, Guid cardId);
    Task CardNumberUsage(string cardNumber);
    Task<CardEntity?> GetCardWithIban(string iban);
    Task<List<CardEntity>?> GetAllCards(string privateNumber);
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
    
    public async Task<List<CardEntity>?> GetAllCards(string privateNumber)
    {
        var accounts = _db.Account.Where(a => a.PrivateNumber == privateNumber);
        if (accounts == null)
        {
            throw new Exception("User does not have accounts");
        }
        var cardAccountConnection = _db.CardAccountConnection
            .Where(c => accounts.Select(a => a.Iban).Contains(c.Iban));
        var cards = await _db.Card
            .Where(c => cardAccountConnection.Select(crd => crd.CardId).Contains(c.Id))
            .ToListAsync();

        return cards;
    }
}