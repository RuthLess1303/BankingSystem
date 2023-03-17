using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;
using InternetBankCore.Validations;

namespace InternetBankCore.Services;

public class CardService
{
    private readonly ICardValidation _cardValidation;
    private readonly IUserRepository _userRepository;

    public CardService(
        IUserRepository userRepository,
        ICardValidation cardValidation)
    {
        _userRepository = userRepository;
        _cardValidation = cardValidation;
    }
    
    public async Task AuthorizeCardAsync(CardAuthorizationRequest request)
    {
        await _cardValidation.AuthorizeCardValidationAsync(request);

        var account = await _userRepository.GetAccountByCardDetails(request.CardNumber, request.PinCode);
        if (account == null)
        {
            throw new ArgumentException("User account not found!", nameof(request));
        }
    }
}