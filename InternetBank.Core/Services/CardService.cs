using System.Runtime.InteropServices.JavaScript;
using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Models;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Newtonsoft.Json;

namespace InternetBank.Core.Services;

public interface ICardService
{
    Task CreateCard(CreateCardRequest request);
    string PrintCardModelProperties((CardModel, string?) model);
    Task<(CardModel, string?)> SeeCard(string iban);
    Task<List<(CardModel,string?)>> SeeAllCards(string privateNumber);
    string PrintAllCardModelProperties(List<(CardModel, string?)> cardModelList);
    string TurnCardInfoToJson(List<(CardModel, string?)> cardModelInfo);
    string TurnCardInfoToJson((CardModel, string?) cardModelInfo);
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
        
        var iban = request.Iban.ToUpper().Replace(" ", "").Replace("-", "");
        
        _propertyValidations.CvvValidation(request.Cvv);
        _propertyValidations.PinValidation(request.Pin);
        _propertyValidations.CheckIbanFormat(iban);
        _propertyValidations.CheckNameOnCard(request.NameOnCard);
        _propertyValidations.CheckCardNumberFormat(request.CardNumber);
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account == null)
        {
            throw new Exception("Iban is not in use");
        }

        var cardEntity = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = request.CardNumber,
            CardHolderName = request.NameOnCard,
            Cvv = request.Cvv,
            Pin = request.Pin,
            ExpirationDate = request.ExpirationDate,
            CreationDate = DateTime.Now
        };

        await _cardRepository.LinkWithAccount(iban, cardEntity.Id);
        await _userRepository.CreateCard(cardEntity);
    }

    public string PrintCardModelProperties((CardModel, string?) cardModel)
    {
        string text = "Your Card Information";

        if (cardModel.Item2 != null)
        {
            text += "\nW A R N I N G\n" +
                    $"{cardModel.Item2}\n\n";
        }
            
        text += $"Card Number: {cardModel.Item1.CardNumber}\n" +
               $"Name on Card: {cardModel.Item1.CardHolderName}\n" +
               $"Cvv: {cardModel.Item1.Cvv}\n" +
               $"Expiration Date: {cardModel.Item1.ExpirationDate}\n";

        return text;
    }
    
    public async Task<(CardModel, string?)> SeeCard(string iban)
    {
        var card = await _cardValidation.GetCardWithIban(iban);
        var isCardExpired = _propertyValidations.IsCardExpired(card.ExpirationDate);
        var cardModel = new CardModel
        {
            CardNumber = card.CardNumber,
            CardHolderName = card.CardHolderName,
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

    public async Task<List<(CardModel,string?)>> SeeAllCards(string privateNumber)
    {
        _propertyValidations.CheckPrivateNumberFormat(privateNumber);
        var user = await _propertyValidations.CheckPrivateNumberUsage(privateNumber);
        if (user == false)
        {
            throw new Exception("Private number not in use");
        }
        
        var cards = await _cardRepository.GetAllCards(privateNumber);
        if (cards == null)
        {
            throw new Exception("There are 0 Cards registered under provided private number");
        }

        var cardModels = new List<(CardModel,string)>();

        foreach (var card in cards)
        {
            var isCardExpired = _propertyValidations.IsCardExpired(card.ExpirationDate);
            
            var cardModel = new CardModel
            {
                CardNumber = card.CardNumber,
                CardHolderName = card.CardHolderName,
                Cvv = card.Cvv,
                ExpirationDate = card.ExpirationDate
            };

            if (isCardExpired)
            {
                cardModels.Add((cardModel, "Card Expired"));
                continue;
            }
            
            if (DateTime.Now.Year == card.ExpirationDate.Year && card.ExpirationDate.Month-DateTime.Now.Month <= 3)
            {
                cardModels.Add((cardModel, $"Card will expire in: {card.ExpirationDate.Month - DateTime.Now.Month} month"));
                continue;
            }
            
            cardModels.Add((cardModel, null));
        }
        
        return cardModels;
    }

    public string PrintAllCardModelProperties(List<(CardModel, string?)> cardModelList)
    {
        string text = "Your Cards Information";

        foreach (var cardModel in cardModelList)
        {
            if (cardModel.Item2 != null)
            {
                text += "\nW A R N I N G\n" +
                        $"{cardModel.Item2}\n\n";
            }
            
            text += $"Card Number: {cardModel.Item1.CardNumber}\n" +
                    $"Name on Card: {cardModel.Item1.CardHolderName}\n" +
                    $"Cvv: {cardModel.Item1.Cvv}\n" +
                    $"Expiration Date: {cardModel.Item1.ExpirationDate}\n\n";
        }

        return text;
    }

    public string TurnCardInfoToJson(List<(CardModel, string?)> cardModelInfo)
    {
        var jsonFormat = JsonConvert.SerializeObject(cardModelInfo);

        return jsonFormat;
    }
    
    public string TurnCardInfoToJson((CardModel, string?) cardModelInfo)
    {
        var jsonFormat = JsonConvert.SerializeObject(cardModelInfo);

        return jsonFormat;
    }
}