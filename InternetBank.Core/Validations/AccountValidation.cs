using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;

namespace InternetBank.Core.Validations;

public interface IAccountValidation
{
    Task OnCreate(CreateAccountRequest request);
    Task<bool> AccountWithIbanExists(string iban);
    Task<bool> IsCurrencySame(string aggressorIban, string receiverIban);
    Task HasSufficientBalance(string iban, decimal amount);
    Task<decimal> GetAmountWithIban(string iban);
    Task<AccountEntity> GetAccountWithIban(string iban);
    Task<bool> HasTransaction(string iban);
    Task<List<TransactionEntity>> GetTransactionsWithIban(string iban);
}

public class AccountValidation : IAccountValidation
{
    private readonly IPropertyValidations _propertyValidations;
    private readonly IAccountRepository _accountRepository;

    public AccountValidation(
        IPropertyValidations propertyValidations, 
        IAccountRepository accountRepository)
    {
        _propertyValidations = propertyValidations;
        _accountRepository = accountRepository;
    }

    public async Task OnCreate(CreateAccountRequest request)
    {
        _propertyValidations.IsAmountValid(request.Amount);
        _propertyValidations.CheckIbanFormat(request.Iban);
        await _propertyValidations.CheckCurrency(request.CurrencyCode);
        await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
    }
    
    public async Task<bool> AccountWithIbanExists(string iban)
    {
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account == null)
        {
            throw new Exception("Account does not exist");
        }

        return true;
    }

    public async Task<bool> IsCurrencySame(string aggressorIban, string receiverIban)
    {
        var aggressorCurrencyCode = await _accountRepository.GetAccountCurrencyCode(aggressorIban);
        var receiverCurrencyCode = await _accountRepository.GetAccountCurrencyCode(receiverIban);

        return string.Equals(aggressorCurrencyCode, receiverCurrencyCode, StringComparison.CurrentCultureIgnoreCase);
    }

    public async Task HasSufficientBalance(string iban, decimal amount)
    {
        var accountMoney = await _accountRepository.GetBalance(iban);
        if (accountMoney < amount)
        {
            throw new Exception("Insufficient balance");
        }
    }
    
    public async Task<decimal> GetAmountWithIban(string iban)
    {
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account == null)
        {
            throw new Exception("Account does not exist");
        }

        return account.Balance;
    }
    
    public async Task<AccountEntity> GetAccountWithIban(string iban)
    {
        _propertyValidations.CheckIbanFormat(iban);
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account == null)
        {
            throw new Exception("Account does not exist");
        }

        return account;
    }

    public async Task<bool> HasTransaction(string iban)
    {
        var transaction = await _accountRepository.HasTransaction(iban);

        return transaction != null;
    }
    
    public async Task<List<TransactionEntity>> GetTransactionsWithIban(string iban)
    {
        var transactionsAsSenderTask = await _accountRepository.GetSenderTransactions(iban);
        var transactionsAsReceiverTask = await _accountRepository.GetReceiverTransactions(iban);

        var allTransactions = new List<TransactionEntity>();
        allTransactions.AddRange(transactionsAsSenderTask);
        allTransactions.AddRange(transactionsAsReceiverTask);

        return allTransactions;
    }

}