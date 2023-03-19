using AtmCore.Repositories;
using AtmCore.Requests;
using AtmCore.Validations;
using BankingSystemSharedDb.Db.Entities;

namespace AtmCore.Services;

public interface ICardAuthService
{
    Task<AccountEntity> GetAuthorizedAccountAsync(WithdrawalRequest request);
}

public class CardAuthService : ICardAuthService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IWithdrawalRequestValidation _requestValidation;

    public CardAuthService(
        IAccountRepository accountRepository,
        ICardRepository cardRepository,
        IWithdrawalRequestValidation requestValidation)
    {
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
        _requestValidation = requestValidation;
    }

    public async Task<AccountEntity> GetAuthorizedAccountAsync(WithdrawalRequest request)
    {
        _requestValidation.ValidatePinCode(request.PinCode);
        _requestValidation.ValidateCreditCardNumber(request.CardNumber);
        _requestValidation.ValidateAmount(request.Amount);
        var card = await _cardRepository.FindCardEntityByCardNumberAsync(request.CardNumber);
        if (card == null) throw new ArgumentException("Card does not exist!", nameof(request.CardNumber));

        if (card.ExpirationDate <= DateTime.UtcNow) throw new UnauthorizedAccessException("Card has expired");

        var account = _accountRepository.GetAccountByCardDetails(request.CardNumber, request.PinCode);

        return account.Result;
    }
}