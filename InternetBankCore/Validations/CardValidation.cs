using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;

namespace InternetBankCore.Validations;

public interface ICardValidation
{
    void OnCreate(CreateCardRequest request);
}

public class CardValidation : ICardValidation
{
    private readonly IPropertyValidations _propertyValidations;
    
    public CardValidation(IPropertyValidations propertyValidations)
    {
        _propertyValidations = propertyValidations;
    }

    public void OnCreate(CreateCardRequest request)
    {
        var cardExpired = _propertyValidations.IsCardExpired(request.ExpirationDate);
        if (cardExpired) throw new Exception("Expiration date should not equal today's date");
        _propertyValidations.CheckCardNumberFormat(request.CardNumber);
    }
}