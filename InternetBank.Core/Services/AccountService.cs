using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;

namespace InternetBank.Core.Services;

public interface IAccountService
{
    Task<string> SeeAccount(string iban);
    Task CreateAccount(CreateAccountRequest request);
}

public class AccountService : IAccountService
{
    private readonly IAccountValidation _accountValidation;
    private readonly IPropertyValidations _propertyValidations;
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionService _transactionService;

    public AccountService(
        IAccountValidation accountValidation, 
        IPropertyValidations propertyValidations, 
        IAccountRepository accountRepository, 
        ITransactionService transactionService)
    {
        _accountValidation = accountValidation;
        _propertyValidations = propertyValidations;
        _accountRepository = accountRepository;
        _transactionService = transactionService;
    }
    
    public async Task CreateAccount(CreateAccountRequest request)
    {
        _propertyValidations.CheckPrivateNumberFormat(request.PrivateNumber);
        var privateNumberCheck = await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
        if (!privateNumberCheck)
        {
            throw new Exception($"User with Private Number: {request.PrivateNumber} is not registered");
        }
        
        var iban = request.Iban.ToUpper().Replace(" ", "").Replace("-", "");
        
        _propertyValidations.CheckIbanFormat(iban);
        await _propertyValidations.CheckIbanUsage(iban);
        await _propertyValidations.CheckCurrency(request.CurrencyCode);
        _propertyValidations.IsAmountValid(request.Amount);

        var accountEntity = CreateAccountEntity(request, iban);
        await _accountRepository.Create(accountEntity);
    }

    private static AccountEntity CreateAccountEntity(CreateAccountRequest request, string iban)
    {
        var accountEntity = new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = request.PrivateNumber,
            Iban = iban,
            CurrencyCode = request.CurrencyCode.ToUpper(),
            Balance = request.Amount,
            CreationDate = DateTime.Now
        };
        return accountEntity;
    }

    public async Task<string> SeeAccount(string iban)
    {
        var account = await GetAccount(iban);
        var text = $"Your Balance is: {account.Item1}\nTransactions\n";

        if (account.Item2 != null)
        {
            foreach (var transaction in account.Item2)
            {
                text += $"{_transactionService.PrintTransaction(transaction)}\n";
            }
        }
        else
        {
            text += "There are no transactions made yet";
        }

        return text;
    }

    private async Task<(decimal,List<TransactionEntity>?)> GetAccount(string iban)
    {
        var balance = await _accountValidation.GetBalanceWithIban(iban);
        var transactionCheck = await _accountValidation.HasTransaction(iban);
        if (!transactionCheck)
        {
            return (balance, null);
        }
        
        var transactions = await _accountValidation.GetTransactionsWithIban(iban);
        
        return (balance, transactions);
    }
}