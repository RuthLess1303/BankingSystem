
using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;
using InternetBankCore.Validations;

namespace InternetBankCore.Services;

public interface ITransactionService
{
    Task MakeTransaction(TransactionRequest request);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrencyService _currencyService;
    private readonly IAccountValidation _accountValidation;
    private readonly IAccountRepository _accountRepository;

    public TransactionService(
        ITransactionRepository transactionRepository, 
        ICurrencyService currencyService, 
        IAccountValidation accountValidation, 
        IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _currencyService = currencyService;
        _accountValidation = accountValidation;
        _accountRepository = accountRepository;
    }

    public async Task MakeTransaction(TransactionRequest request)
    {
        await _accountValidation.AccountWithIbanExists(request.AggressorIban);
        await _accountValidation.HasSufficientBalance(request.AggressorIban, request.Amount);
        await _accountValidation.AccountWithIbanExists(request.ReceiverIban);
        var aggressorCurrency = _accountRepository.GetAccountCurrencyCode(request.AggressorIban);
        var receiverCurrency = _accountRepository.GetAccountCurrencyCode(request.ReceiverIban);
        var convertedAmount = await _currencyService.ConvertAmount(aggressorCurrency.Result, receiverCurrency.Result, request.Amount);
        request.Amount = convertedAmount;
        var transactionEntity = new TransactionEntity
        {
            Amount = request.Amount,
            AggressorIban = request.AggressorIban,
            ReceiverIban = request.ReceiverIban,
            CurrencyCode = receiverCurrency.Result,
            Rate = await _currencyService.GetRate(receiverCurrency.Result),
        };

        if (request.ReceiverIban.Contains("CD"))
        {
            await _transactionRepository.MakeTransaction(request);
            transactionEntity.Type = "Inner";
            transactionEntity.Fee = 0;
            transactionEntity.GrossAmount = request.Amount;
            transactionEntity.TransactionTime = DateTime.Now;
            await _transactionRepository.AddDataInDb(transactionEntity);
            return;
        }
        await _transactionRepository.MakeTransactionWithFee(request);
        transactionEntity.Type = "Outside";
        transactionEntity.Fee = request.Amount / 100 + (decimal)0.5;
        transactionEntity.GrossAmount = request.Amount * (decimal)1.01 + (decimal)0.5;
        transactionEntity.TransactionTime = DateTime.Now;
        await _transactionRepository.AddDataInDb(transactionEntity);
    }
}