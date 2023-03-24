using InternetBank.Db.Requests;

namespace InternetBankCore.Validations;

public interface ICardValidation
{
    Task OnCreate(CreateCardRequest request);
}

public class CardValidation : ICardValidation
{
    private readonly IPropertyValidations _propertyValidations;
    
    public CardValidation(IPropertyValidations propertyValidations)
    {
        _propertyValidations = propertyValidations;
    }

    public async Task OnCreate(CreateCardRequest request)
    {
        var cardExpired = _propertyValidations.IsCardExpired(request.ExpirationDate);
        if (cardExpired) throw new Exception("Expiration date should not equal today's date");
        await _propertyValidations.CheckCardNumberFormat(request.CardNumber);
    }
}