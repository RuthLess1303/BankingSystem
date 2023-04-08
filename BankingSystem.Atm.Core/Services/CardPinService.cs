using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Requests;
using BankingSystem.Atm.Core.Validations;

namespace BankingSystem.Atm.Core.Services;

public interface ICardPinService
{
    Task ChangeCardPin(ChangePinRequest request);
}

public class CardCardPinService : ICardPinService
{
    private readonly ICardAuthService _cardAuthService;
    private readonly ICardRepository _cardRepository;
    private readonly IPinRepository _pinRepository;
    private readonly IWithdrawalRequestValidation _requestValidation;

    public CardCardPinService(
        ICardRepository cardRepository,
        ICardAuthService cardAuthService,
        IPinRepository pinRepository,
        IWithdrawalRequestValidation requestValidation)
    {
        _cardRepository = cardRepository;
        _cardAuthService = cardAuthService;
        _pinRepository = pinRepository;
        _requestValidation = requestValidation;
    }

    public async Task ChangeCardPin(ChangePinRequest request)
    {
        _requestValidation.ValidatePinCode(request.NewPin);

        var account = await _cardAuthService.GetAuthorizedAccountAsync(request.CardNumber, request.PinCode);
        if (account == null)
            throw new ArgumentException("Account not found with the given CardNumber.", nameof(request.CardNumber));
        // Get the card associated with the provided card number
        var card = await _cardRepository.FindCardEntityByCardNumberAsync(request.CardNumber);
        if (card == null)
            throw new ArgumentException("Card not found with the given card number.", nameof(request.CardNumber));

        // Verify that the provided PIN matches the current PIN for the card
        if (request.PinCode != card.Pin) throw new ArgumentException("Invalid PIN code.", nameof(request.PinCode));

        if (request.NewPin == card.Pin) throw new Exception("New PIN cannot be same as the current PIN!");
        // Change the PIN for the card
        await _pinRepository.ChangePinInDb(card, request.NewPin);
    }
}