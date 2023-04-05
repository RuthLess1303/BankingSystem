using BankingSystem.Atm.Core.Repositories;
using BankingSystem.Atm.Core.Requests;
using BankingSystem.Atm.Core.Validations;
using InternetBank.Db.Db.Entities;

namespace BankingSystem.Atm.Core.Services;

public interface IWithdrawalService
{
    Task Withdraw(WithdrawalRequest request);
}

public class WithdrawalService : IWithdrawalService
{
    private readonly ICardAuthService _cardAuthService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IWithdrawalRequestValidation _requestValidation;
    
    private const decimal WithdrawalFeePercentage = 0.02m;
    private const decimal DailyLimitInGel = 10000;
    private const decimal DailyLimitInUsd = 4000;
    private const decimal DailyLimitInEur = 3500;

    public WithdrawalService(
        ICardAuthService cardAuthService,
        ITransactionRepository transactionRepository,
        IWithdrawalRequestValidation requestValidation)
    {
        _cardAuthService = cardAuthService;
        _transactionRepository = transactionRepository;
        _requestValidation = requestValidation;
    }

    public async Task Withdraw(WithdrawalRequest request)
    {
        var account = await _cardAuthService.GetAuthorizedAccountAsync(request.CardNumber, request.PinCode) 
                      ?? throw new ArgumentException("Account not found with the given CardNumber.", nameof(request.CardNumber));

        _requestValidation.ValidateAmount(request.Amount);
        
        // Calculate the withdrawal fee
        var fee = request.Amount * WithdrawalFeePercentage;
        var withdrawAmount = request.Amount + fee;

        if (account.Balance < withdrawAmount) throw new InvalidOperationException("Insufficient funds.");

        // Check if the withdrawal amount exceeds the daily limit
        var dailyLimit = account.CurrencyCode.ToUpper() switch
        {
            "GEL" => DailyLimitInGel,
            "USD" => DailyLimitInUsd,
            "EUR" => DailyLimitInEur,
            _ => throw new Exception("Unsupported currency.")
        };

        var withdrawalsInLast24Hours = await _transactionRepository.GetWithdrawalAmountInLast24HoursAsync(account.Iban);

        var totalWithdrawals = withdrawalsInLast24Hours + request.Amount;

        if (totalWithdrawals > dailyLimit)
            throw new InvalidOperationException($"Withdrawals are limited to {dailyLimit} GEL within 24 hours.");

        // Update the account balance
        account.Balance -= withdrawAmount;

        // Add a withdrawal record
        var withdrawal = new TransactionEntity
        {
            SenderIban = account.Iban,
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