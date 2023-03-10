using InternetBankCore.Db.Entities;
using InternetBankCore.Db.Repositories;
using InternetBankCore.Requests;

namespace InternetBankCore.Validations;

public interface IAccountValidation
{
    Task OnCreate(CreateAccountRequest request);
    Task AccountWithIbanExists(string iban);
    Task<bool> IsCurrencySame(string aggressorIban, string receiverIban);
    Task HasSufficientBalance(string iban, decimal amount);
    Task<decimal> GetAmountWithIban(string iban);
    Task<AccountEntity?> GetAccountWithIban(string iban);
    Task<bool> HasTransaction(string iban);
    Task<List<TransactionEntity>> GetTransactionsWithIban(string iban);
    Task<CardEntity> GetCardWithIban(string iban);
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
        _propertyValidations.CheckAmount(request.Amount);
        _propertyValidations.CheckIbanFormat(request.Iban);
        await _propertyValidations.CheckCurrency(request.CurrencyCode);
        await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
    }
    
    public async Task AccountWithIbanExists(string iban)
    {
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account == null)
        {
            throw new Exception("Account does not exist");
        }
    }

    public async Task<bool> IsCurrencySame(string aggressorIban, string receiverIban)
    {
        var aggressorCurrencyCode = await _accountRepository.GetAccountCurrencyCode(aggressorIban);
        var receiverCurrencyCode = await _accountRepository.GetAccountCurrencyCode(receiverIban);

        if (aggressorCurrencyCode == receiverCurrencyCode)
        {
            return true;
        }

        return false;
    }

    public async Task HasSufficientBalance(string iban, decimal amount)
    {
        var accountMoney = await _accountRepository.GetAccountMoney(iban);
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

        return account.Amount;
    }
    
    public async Task<AccountEntity?> GetAccountWithIban(string iban)
    {
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

        if (transaction == null)
        {
            return false;
        }

        return true;
    }

    public async Task<List<TransactionEntity>> GetTransactionsWithIban(string iban)
    {
        var allTransactions = new List<TransactionEntity>();
        
        var transactionsAsAggressor = await _accountRepository.GetAggressorTransactions(iban);
        var transactionsAsReceiver = await _accountRepository.GetReceiverTransactions(iban);
        
        allTransactions.AddRange(transactionsAsAggressor);
        allTransactions.AddRange(transactionsAsReceiver);

        return allTransactions;
    }

    public async Task<CardEntity> GetCardWithIban(string iban)
    {
        var card = await _accountRepository.GetCardWithIban(iban);
        if (card == null)
        {
            throw new Exception("There are 0 cards registered under provided Iban");
        }

        return card;
    }
}