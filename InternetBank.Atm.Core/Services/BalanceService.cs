using InternetBank.Atm.Core.Repositories;
using InternetBank.Atm.Core.Requests;

namespace InternetBank.Atm.Core.Services;

public interface IBalanceService
{
    Task<decimal> SeeBalance(AuthorizeCardRequest request);
}

public class BalanceService : IBalanceService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICardAuthService _cardAuthService;

    public BalanceService(
        IAccountRepository accountRepository,
        ICardAuthService cardAuthService)
    {
        _accountRepository = accountRepository;
        _cardAuthService = cardAuthService;
    }

    public async Task<decimal> SeeBalance(AuthorizeCardRequest request)
    {
        var account = await _cardAuthService.GetAuthorizedAccountAsync(request.CardNumber, request.PinCode);
        if (account == null)
        {
            throw new ArgumentException($"Invalid card number or PIN code");
        }
        
        var balance = await _accountRepository.GetAccountBalance(account.Iban);
        return balance;
    }
}