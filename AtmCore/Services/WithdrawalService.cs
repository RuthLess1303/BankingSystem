using AtmCore.Repositories;
using AtmCore.Requests;
using BankingSystemSharedDb.Db.Entities;

namespace AtmCore.Services;

public interface IWithdrawalService
{
    Task Withdraw(WithdrawalRequest request);
}

public class WithdrawalService : IWithdrawalService
{
    private readonly ICardAuthService _cardAuthService;
    private readonly ITransactionRepository _transactionRepository;

    public WithdrawalService(
        ICardAuthService cardAuthService,
        ITransactionRepository transactionRepository)
    {
        _cardAuthService = cardAuthService;
        _transactionRepository = transactionRepository;
    }

    public async Task Withdraw(WithdrawalRequest request)
    {
        var account = await _cardAuthService.GetAuthorizedAccountAsync(request.CardNumber, request.PinCode);
        if (account == null)
            throw new ArgumentException("Account not found with the given CardNumber.", nameof(request.CardNumber));


        if (request.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(request.CardNumber));

        // Calculate the withdrawal fee
        var fee = request.Amount * 0.02m;
        var withdrawAmount = request.Amount + fee;

        if (account.Balance < withdrawAmount) throw new InvalidOperationException("Insufficient funds.");

        // Check if the withdrawal amount exceeds the daily limit
        // const decimal dailyLimit = 10000m;
        var dailyLimit = account.CurrencyCode switch
        {
            "GEL" => 10000,
            "USD" => 4000,
            "EUR" => 3500,
            _ => throw new Exception("Unsupported currency.")
        };

        var withdrawalsInLast24Hours = await _transactionRepository.GetWithdrawalsInLast24HoursAsync(account.Iban);

        var totalWithdrawals = withdrawalsInLast24Hours + request.Amount;

        if (totalWithdrawals > dailyLimit)
            throw new InvalidOperationException($"Withdrawals are limited to {dailyLimit} GEL within 24 hours.");

        // Update the account balance
        account.Balance -= withdrawAmount;

        // Add a withdrawal record
        var withdrawal = new TransactionEntity
        {
            AggressorIban = account.Iban,
            ReceiverIban = account.Iban,
            Amount = request.Amount,
            Fee = fee,
            Type = "ATM",
            TransactionTime = DateTime.UtcNow,
            GrossAmount = withdrawAmount,
            CurrencyCode = account.CurrencyCode,
            Rate = 1
        };

        await _transactionRepository.AddTransactionInDb(withdrawal);
    }
}