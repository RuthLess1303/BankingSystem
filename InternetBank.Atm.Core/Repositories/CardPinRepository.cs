using InternetBank.Db.Db;
using InternetBank.Db.Db.Entities;

namespace InternetBank.Atm.Core.Repositories;

public interface IPinRepository
{
    Task ChangePinInDb(CardEntity card, string newPin);
}

public class CardPinRepository : IPinRepository
{
    private readonly AppDbContext _db;

    public CardPinRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task ChangePinInDb(CardEntity card, string newPin)
    {
        card.Pin = newPin;
        await _db.SaveChangesAsync();
    }
}