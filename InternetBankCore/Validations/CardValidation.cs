using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;

namespace InternetBankCore.Validations;

public interface ICardValidation
{
    Task OnCreate(CreateCardRequest request);
    Task<CardEntity> GetCardWithIban(string iban);
}

public class CardValidation : ICardValidation
{
    private readonly IPropertyValidations _propertyValidations;
    private readonly IAccountRepository _accountRepository;
    
    public CardValidation(
        IPropertyValidations propertyValidations, 
        IAccountRepository accountRepository)
    {
        _propertyValidations = propertyValidations;
        _accountRepository = accountRepository;
    }

    public async Task OnCreate(CreateCardRequest request)
    {
        var cardExpired = _propertyValidations.IsCardExpired(request.ExpirationDate);
        if (cardExpired) throw new Exception("Expiration date should not equal today's date");
        await _propertyValidations.CheckCardNumberFormat(request.CardNumber);
    }
    
    public async Task<CardEntity> GetCardWithIban(string iban)
    {
        var card = await _accountRepository.GetCardWithIban(iban);
        if (card == null)
        {
            throw new Exception("There are 0 cards registered under provided Iban");
        }

        return card;
    }
}