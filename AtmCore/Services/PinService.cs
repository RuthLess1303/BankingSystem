using AtmCore.Repositories;
using AtmCore.Requests;

namespace AtmCore.Services;

public class PinService
{
    private readonly CardAuthService _cardAuthService;
    private readonly ICardRepository _cardRepository;
    private readonly IPinRepository _pinRepository;

    public PinService(
        ICardRepository cardRepository,
        CardAuthService cardAuthService,
        IPinRepository pinRepository)
    {
        _cardRepository = cardRepository;
        _cardAuthService = cardAuthService;
        _pinRepository = pinRepository;
    }

    public async Task ChangeCardPin(WithdrawalRequest request, string newPin)
    {
        var account = await _cardAuthService.GetAuthorizedAccountAsync(request);
        if (account == null)
            throw new ArgumentException("Account not found with the given CardNumber.", nameof(request.CardNumber));
        // Get the card associated with the provided card number
        var card = await _cardRepository.FindCardEntityByCardNumberAsync(request.CardNumber);
        if (card == null)
            throw new ArgumentException("Card not found with the given card number.", nameof(request.CardNumber));

        // Verify that the provided PIN matches the current PIN for the card
        if (request.PinCode != card.Pin) throw new ArgumentException("Invalid PIN code.", nameof(request.PinCode));

        // Change the PIN for the card
        await _pinRepository.ChangePinInDb(card, newPin);
    }
}