using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;

namespace InternetBank.Core.Services;

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
    private readonly ITransactionValidations _transactionValidations;
    private readonly IUserRepository _userRepository;

    public TransactionService(
        ITransactionRepository transactionRepository, 
        ICurrencyService currencyService, 
        IAccountValidation accountValidation,
        ITransactionValidations transactionValidations, 
        IUserRepository userRepository)
    {
        _transactionRepository = transactionRepository;
        _currencyService = currencyService;
        _accountValidation = accountValidation;
        _transactionValidations = transactionValidations;
        _userRepository = userRepository;
    }

    public async Task MakeTransaction(TransactionRequest request)
    {
        var senderAccount = await _accountValidation.GetAccountWithIban(request.SenderIban);
        var receiverAccount = await _accountValidation.GetAccountWithIban(request.ReceiverIban);
        if (request.Amount > senderAccount.Balance)
        {
            throw new Exception("Insufficient funds");
        }
        
        var senderCurrency = senderAccount.CurrencyCode;
        var receiverCurrency = receiverAccount.CurrencyCode;
        var convertedAmount = await _currencyService.ConvertAmount(senderCurrency, receiverCurrency, request.Amount);
        var transactionEntity = new TransactionEntity
        {
            Amount = convertedAmount,
            SenderIban = request.SenderIban,
            ReceiverIban = request.ReceiverIban,
            CurrencyCode = receiverCurrency,
            Rate = await _currencyService.GetRateAsync(receiverCurrency),
        };
        var senderUser = await _userRepository.GetUserWithIban(request.SenderIban);
        var receiverUser = await _userRepository.GetUserWithIban(request.ReceiverIban);

        if (senderUser.Id == receiverUser.Id)
        {
            await _transactionRepository.MakeTransaction(request, convertedAmount);
            transactionEntity.Type = "Inner";
            transactionEntity.Fee = 0;
            transactionEntity.GrossAmount = request.Amount;
            transactionEntity.TransactionTime = DateTimeOffset.Now;
            await _transactionRepository.AddDataInDb(transactionEntity);
            return;
        }

        var fee = _transactionValidations.CalculateFee(request.Amount, 1, (decimal)0.5);
        var grossAmount = request.Amount + fee;
        if (grossAmount > senderAccount.Balance)
        {
            throw new Exception("Insufficient funds");
        }
        
        await _transactionRepository.MakeTransactionWithFee(request, convertedAmount);
        transactionEntity.Type = "Outside";
        transactionEntity.Fee = fee;
        transactionEntity.GrossAmount = grossAmount;
        transactionEntity.TransactionTime = DateTimeOffset.Now;
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