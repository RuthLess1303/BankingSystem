using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;

namespace InternetBank.Core.Validations;

public interface ICardValidation
{
    Task OnCreate(CreateCardRequest request);
    Task<CardEntity> GetCardWithIban(string iban);
}

public class CardValidation : ICardValidation
{
    private readonly IPropertyValidations _propertyValidations;
    private readonly ICardRepository _cardRepository;
    
    public CardValidation(
        IPropertyValidations propertyValidations, 
        ICardRepository cardRepository)
    {
        _propertyValidations = propertyValidations;
        _cardRepository = cardRepository;
    }

    public Task OnCreate(CreateCardRequest request)
    {
        var cardExpired = _propertyValidations.IsCardExpired(request.ExpirationDate);
        if (cardExpired) throw new Exception("Expiration date should not equal today's date");
        var cardCheck = _propertyValidations.CheckCardNumberFormat(request.CardNumber);
        if (!cardCheck)
        {
            throw new Exception("Card Number is not valid!");
        }

        return Task.CompletedTask;
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