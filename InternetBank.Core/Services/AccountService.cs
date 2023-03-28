using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Models;

namespace InternetBank.Core.Services;

public interface IAccountService
{
    Task<(decimal, List<TransactionEntity>?)> SeeAccount(string iban);
    Task<(CardModel, string?)> SeeCard(string iban);
}

public class AccountService : IAccountService
{
    private readonly IAccountValidation _accountValidation;
    private readonly IPropertyValidations _propertyValidations;

    public AccountService(
        IAccountValidation accountValidation, 
        IPropertyValidations propertyValidations)
    {
        _accountValidation = accountValidation;
        _propertyValidations = propertyValidations;
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
}