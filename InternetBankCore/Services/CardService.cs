using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Models;
using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;
using InternetBankCore.Validations;

namespace InternetBankCore.Services;

public interface ICardService
{
    Task CreateCard(CreateCardRequest request);
    string PrintCardModelProperties(CardModel model);
    Task<(CardModel, string?)> SeeCard(string iban);
}

public class CardService : ICardService
{
    private readonly ICardValidation _cardValidation;
    private readonly IPropertyValidations _propertyValidations;
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;

    public CardService(
        ICardValidation cardValidation, 
        IPropertyValidations propertyValidations, 
        IAccountRepository accountRepository, 
        ICardRepository cardRepository, 
        IUserRepository userRepository)
    {
        _cardValidation = cardValidation;
        _propertyValidations = propertyValidations;
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
        _userRepository = userRepository;
    }
    
    public async Task CreateCard(CreateCardRequest request)
    {
        if (request.ExpirationDate <= DateTime.Now || request.ExpirationDate.Year <= DateTime.Now.Year)
        {
            throw new Exception("Expiration date must be more than 1 year apart");
        }
        _propertyValidations.CvvValidation(request.Cvv);
        _propertyValidations.PinValidation(request.Pin);
        _propertyValidations.CheckIbanFormat(request.Iban);
        _propertyValidations.CheckNameOnCard(request.NameOnCard);
        await _propertyValidations.CheckCardNumberFormat(request.CardNumber);
        var account = _accountRepository.GetAccountWithIban(request.Iban);
        if (account == null)
        {
            throw new Exception("Iban is not in use");
        }
        
        
        var cardEntity = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = request.CardNumber,
            NameOnCard = request.NameOnCard,
            Cvv = request.Cvv,
            Pin = request.Pin,
            ExpirationDate = request.ExpirationDate,
            CreationDate = DateTime.Now
        };

        await _cardRepository.LinkWithAccount(request.Iban, cardEntity.Id);
        await _userRepository.CreateCard(cardEntity);
    }

    public string PrintCardModelProperties(CardModel model)
    {
        return $"Card Number: {model.CardNumber}\n" +
               $"Name on Card: {model.NameOnCard}\n" +
               $"Cvv: {model.Cvv}\n" +
               $"Expiration Date: {model.ExpirationDate}\n";
    }
    
    public async Task<(CardModel, string?)> SeeCard(string iban)
    {
        var card = await _cardValidation.GetCardWithIban(iban);
        var isCardExpired = _propertyValidations.IsCardExpired(card.ExpirationDate);
        var cardModel = new CardModel
        {
            CardNumber = card.CardNumber,
            NameOnCard = card.NameOnCard,
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