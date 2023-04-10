using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;

namespace InternetBank.Core.Validations;

public interface ICardValidation
{
    Task<CardEntity> GetCardWithIban(string iban);
}

public class CardValidation : ICardValidation
{
    private readonly ICardRepository _cardRepository;
    
    public CardValidation(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }
    
    public async Task<CardEntity> GetCardWithIban(string iban)
    {
        var card = await _cardRepository.GetCardWithIban(iban);
        if (card == null)
        {
            throw new Exception("There are 0 cards registered under provided Iban");
        }

        return card;
    }
}