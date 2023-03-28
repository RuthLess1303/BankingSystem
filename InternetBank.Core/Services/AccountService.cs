using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;

namespace InternetBank.Core.Services;

public interface IAccountService
{
    Task<(decimal, List<TransactionEntity>?)> SeeAccount(string iban);
    Task CreateAccount(CreateAccountRequest request);
}

public class AccountService : IAccountService
{
    private readonly IAccountValidation _accountValidation;
    private readonly IPropertyValidations _propertyValidations;
    private readonly IAccountRepository _accountRepository;

    public AccountService(
        IAccountValidation accountValidation, 
        IPropertyValidations propertyValidations, 
        IAccountRepository accountRepository)
    {
        _accountValidation = accountValidation;
        _propertyValidations = propertyValidations;
        _accountRepository = accountRepository;
    }
    
    public async Task CreateAccount(CreateAccountRequest request)
    {
        _propertyValidations.CheckPrivateNumberFormat(request.PrivateNumber);
        var privateNumberCheck = await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
        if (!privateNumberCheck)
        {
            throw new Exception($"User with Private Number: {request.PrivateNumber} is not registered");
        }
        _propertyValidations.CheckIbanFormat(request.Iban);
        await _propertyValidations.CheckIbanUsage(request.Iban);
        await _propertyValidations.CheckCurrency(request.CurrencyCode);

        var accountEntity = new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = request.PrivateNumber,
            Iban = request.Iban,
            CurrencyCode = request.CurrencyCode,
            Balance = request.Amount,
            CreationDate = DateTime.Now
        };

        await _accountRepository.Create(accountEntity);
    }

    public async Task<(decimal,List<TransactionEntity>?)> SeeAccount(string iban)
    {
        var balance = await _accountValidation.GetAmountWithIban(iban);
        var transactionCheck = await _accountValidation.HasTransaction(iban);
        if (!transactionCheck)
        {
            return (balance, null);
        }
        
        var transactions = await _accountValidation.GetTransactionsWithIban(iban);
        return (balance, transactions);
    }
}