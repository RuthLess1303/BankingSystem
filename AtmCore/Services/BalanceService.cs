using AtmCore.Repositories;
using AtmCore.Requests;

namespace AtmCore.Services;

public class BalanceService
{
    private readonly IAccountRepository _accountRepository;
    private readonly CardAuthService _cardAuthService;

    public BalanceService(
        IAccountRepository accountRepository,
        CardAuthService cardAuthService)
    {
        _accountRepository = accountRepository;
        _cardAuthService = cardAuthService;
    }

    public async Task<decimal> SeeBalance(WithdrawalRequest request)
    {
        var account = await _cardAuthService.GetAuthorizedAccountAsync(request);
        var balance = await _accountRepository.GetAccountMoney(account.Iban);
        return balance;
    }
}