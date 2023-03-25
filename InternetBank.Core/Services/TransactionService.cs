using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using InternetBankCore.Validations;

namespace InternetBankCore.Services;

public interface ITransactionService
{
    Task MakeTransaction(TransactionRequest request);
    string PrintTransaction(TransactionEntity transaction);
}

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICurrencyService _currencyService;
    private readonly IAccountValidation _accountValidation;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionValidations _transactionValidations;

    public TransactionService(
        ITransactionRepository transactionRepository, 
        ICurrencyService currencyService, 
        IAccountValidation accountValidation, 
        IAccountRepository accountRepository, 
        ITransactionValidations transactionValidations)
    {
        _transactionRepository = transactionRepository;
        _currencyService = currencyService;
        _accountValidation = accountValidation;
        _accountRepository = accountRepository;
        _transactionValidations = transactionValidations;
    }

    public async Task MakeTransaction(TransactionRequest request)
    {
        await _accountValidation.AccountWithIbanExists(request.SenderIban);
        await _accountValidation.HasSufficientBalance(request.SenderIban, request.Amount);
        await _accountValidation.AccountWithIbanExists(request.ReceiverIban);
        var aggressorCurrency = await _accountRepository.GetAccountCurrencyCode(request.SenderIban);
        var receiverCurrency = await _accountRepository.GetAccountCurrencyCode(request.ReceiverIban);
        var convertedAmount = await _currencyService.ConvertAmount(aggressorCurrency, receiverCurrency, request.Amount);
        var transactionEntity = new TransactionEntity
        {
            Amount = convertedAmount,
            SenderIban = request.SenderIban,
            ReceiverIban = request.ReceiverIban,
            CurrencyCode = receiverCurrency,
            Rate = await _currencyService.GetRateAsync(receiverCurrency),
        };
    
        if (request.ReceiverIban.Contains("CD"))
        {
            await _transactionRepository.MakeTransaction(request, convertedAmount);
            transactionEntity.Type = "Inner";
            transactionEntity.Fee = 0;
            transactionEntity.GrossAmount = request.Amount;
            transactionEntity.TransactionTime = DateTime.Now;
            await _transactionRepository.AddDataInDb(transactionEntity);
            return;
        }
        await _transactionRepository.MakeTransactionWithFee(request, convertedAmount);
        transactionEntity.Type = "Outside";
        transactionEntity.Fee = _transactionValidations.CalculateFee(request.Amount, 1,(decimal)0.5);
        transactionEntity.GrossAmount = _transactionValidations.CalculateGrossAmount(request.Amount,1,(decimal)0.5);
        transactionEntity.TransactionTime = DateTime.Now;
        await _transactionRepository.AddDataInDb(transactionEntity);
    }
    
    public string PrintTransaction(TransactionEntity transaction)
    {
        return $"Transaction Amount: {transaction.Amount}\n" +
               $"Sender's Iban: {transaction.SenderIban}\n" +
               $"Receiver's Iban: {transaction.ReceiverIban}\n" +
               $"Amount With Fee: {transaction.GrossAmount}\n" +
               $"Currency: {transaction.CurrencyCode}\n" +
               $"Currency Rate: {transaction.Rate}\n" +
               $"Transaction Type: {transaction.Type}\n" +
               $"Transaction Time: {transaction.TransactionTime}\n";
    }    
}