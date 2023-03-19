using BankingSystemSharedDb.Db;
using BankingSystemSharedDb.Db.Entities;

namespace AtmCore.Repositories;

public interface IPinRepository
{
    Task ChangePinInDb(CardEntity card, string newPin);
}

public class PinRepository : IPinRepository
{
    private readonly AppDbContext _db;

    public PinRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task ChangePinInDb(CardEntity card, string newPin)
    {
        card.Pin = newPin;
        await _db.SaveChangesAsync();
    }
}