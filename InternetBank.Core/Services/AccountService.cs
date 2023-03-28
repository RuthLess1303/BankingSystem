<<<<<<< HEAD:InternetBankCore/Services/AccountService.cs
using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Models;
using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;
using InternetBankCore.Validations;
=======
using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Models;
>>>>>>> 8aea98499f6e1072c2bed6b80e900095f37d6c23:InternetBank.Core/Services/AccountService.cs

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
<<<<<<< HEAD:InternetBankCore/Services/AccountService.cs
=======

    public async Task<(CardModel, string?)> SeeCard(string iban)
    {
        var card = await _accountValidation.GetCardWithIban(iban);
        var isCardExpired = _propertyValidations.IsCardExpired(card.ExpirationDate);
        var cardModel = new CardModel
        {
            CardNumber = card.CardNumber,
            NameOnCard = card.CardHolderName,
            Cvv = card.Cvv,
            ExpirationDate = card.ExpirationDate
        };

        if (isCardExpired)
        {
            return (cardModel, "Card is Expired");
        }

        if (DateTime.Now.Year == card.ExpirationDate.Year && card.ExpirationDate.Month-DateTime.Now.Month <= 3)
        {
            return (cardModel, $"Card will expire in: {card.ExpirationDate.Month - DateTime.Now.Month} month");
        }

        return (cardModel, null);
    }
>>>>>>> 8aea98499f6e1072c2bed6b80e900095f37d6c23:InternetBank.Core/Services/AccountService.cs
}