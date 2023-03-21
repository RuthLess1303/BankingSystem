using AtmCore.Repositories;
using AtmCore.Requests;

namespace AtmCore.Services;

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
        
        var balance = await _accountRepository.GetAccountMoney(account.Iban);
        return balance;
    }
}