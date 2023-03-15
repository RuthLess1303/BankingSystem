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

    // public async Task MakeTransaction(TransactionRequest request)
    // {
    //     await _accountValidation.AccountWithIbanExists(request.AggressorIban);
    //     await _accountValidation.HasSufficientBalance(request.AggressorIban, request.Amount);
    //     await _accountValidation.AccountWithIbanExists(request.ReceiverIban);
    //     var aggressorCurrency = _accountRepository.GetAccountCurrencyCode(request.AggressorIban);
    //     var receiverCurrency = _accountRepository.GetAccountCurrencyCode(request.ReceiverIban);
    //     var convertedAmount = await _currencyService.ConvertAmount(aggressorCurrency.Result, receiverCurrency.Result, request.Amount);
    //     request.Amount = convertedAmount;
    //     var transactionEntity = new TransactionEntity
    //     {
    //         Amount = request.Amount,
    //         AggressorIban = request.AggressorIban,
    //         ReceiverIban = request.ReceiverIban,
    //         CurrencyCode = receiverCurrency.Result,
    //         Rate = await _currencyService.GetCurrencyAsync(receiverCurrency.Result),
    //     };
    //
    //     if (request.ReceiverIban.Contains("CD"))
    //     {
    //         await _transactionRepository.MakeTransaction(request);
    //         transactionEntity.Type = "Inner";
    //         transactionEntity.Fee = 0;
    //         transactionEntity.GrossAmount = request.Amount;
    //         transactionEntity.TransactionTime = DateTime.Now;
    //         await _transactionRepository.AddDataInDb(transactionEntity);
    //         return;
    //     }
    //     await _transactionRepository.MakeTransactionWithFee(request);
    //     transactionEntity.Type = "Outside";
    //     transactionEntity.Fee = request.Amount / 100 + (decimal)0.5;
    //     transactionEntity.GrossAmount = request.Amount * (decimal)1.01 + (decimal)0.5;
    //     transactionEntity.TransactionTime = DateTime.Now;
    //     await _transactionRepository.AddDataInDb(transactionEntity);
    // }
    public async Task MakeTransaction(TransactionRequest request)
        {
            await ValidateRequest(request);
            var aggressorCurrency = await _accountRepository.GetAccountCurrencyCode(request.AggressorIban);
            var receiverCurrency = await _accountRepository.GetAccountCurrencyCode(request.ReceiverIban);
            var convertedAmount = await _currencyService.ConvertAmount(aggressorCurrency, receiverCurrency, request.Amount);
            request.Amount = convertedAmount;
            var transactionEntity = await CreateTransactionEntity(request, receiverCurrency);
            await SaveTransaction(transactionEntity, request);
        }

        private async Task ValidateRequest(TransactionRequest request)
        {
            await _accountValidation.AccountWithIbanExists(request.AggressorIban);
            await _accountValidation.HasSufficientBalance(request.AggressorIban, request.Amount);
            await _accountValidation.AccountWithIbanExists(request.ReceiverIban);
        }

        private async Task<TransactionEntity> CreateTransactionEntity(TransactionRequest request, string receiverCurrency)
        {
            var transactionEntity = new TransactionEntity
            {
                Amount = request.Amount,
                AggressorIban = request.AggressorIban,
                ReceiverIban = request.ReceiverIban,
                CurrencyCode = receiverCurrency,
                TransactionTime = DateTime.Now
            };

            if (request.ReceiverIban.Contains("CD"))
            {
                transactionEntity.Type = "Inner";
                transactionEntity.Fee = 0;
                transactionEntity.GrossAmount = request.Amount;
                transactionEntity.Rate = await _currencyService.GetCurrencyAsync(receiverCurrency);
            }
            else
            {
                transactionEntity.Type = "Outside";
                transactionEntity.Fee = request.Amount / 100 + (decimal)0.5;
                transactionEntity.GrossAmount = request.Amount * (decimal)1.01 + (decimal)0.5;
                transactionEntity.Rate = await _currencyService.GetCurrencyAsync(receiverCurrency);
            }

            return transactionEntity;
        }

        private async Task SaveTransaction(TransactionEntity transactionEntity, TransactionRequest request)
        {
            if (request.ReceiverIban.Contains("CD"))
            {
                await _transactionRepository.MakeTransaction(request);
            }
            else
            {
                await _transactionRepository.MakeTransactionWithFee(request);
            }

            await _transactionRepository.AddDataInDb(transactionEntity);
        }
}