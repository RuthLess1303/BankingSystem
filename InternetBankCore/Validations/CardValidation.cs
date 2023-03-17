using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;

namespace InternetBankCore.Validations;

public interface ICardValidation
{
    void OnCreate(CreateCardRequest request);
    Task AuthorizeCardValidationAsync(CardAuthorizationRequest request);
}

public class CardValidation : ICardValidation
{
    private readonly ICardRepository _cardRepository;
    private readonly IPropertyValidations _propertyValidations;
    private readonly IUserRepository _userRepository;

    public CardValidation(IPropertyValidations propertyValidations, ICardRepository cardRepository,
        IUserRepository userRepository)
    {
        _propertyValidations = propertyValidations;
        _cardRepository = cardRepository;
        _userRepository = userRepository;
    }

    public void OnCreate(CreateCardRequest request)
    {
        var cardExpired = _propertyValidations.IsCardExpired(request.ExpirationDate);
        if (cardExpired) throw new Exception("Expiration date should not equal today's date");
        _propertyValidations.CheckCardNumberFormat(request.CardNumber);
    }

    public async Task AuthorizeCardValidationAsync(CardAuthorizationRequest request)
    {
        var card = await _cardRepository.FindCardEntityByCardNumberAsync(request.CardNumber);

        if (card.ExpirationDate <= DateTime.UtcNow) throw new UnauthorizedAccessException("Card has expired");

        var account = await _userRepository.GetAccountByCardDetails(card.CardNumber, card.Pin);
    }
}