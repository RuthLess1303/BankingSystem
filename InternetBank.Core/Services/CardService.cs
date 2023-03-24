using InternetBank.Db.Db.Models;

namespace InternetBankCore.Services;

public interface ICardService
{
    string PrintCardModelProperties(CardModel model);
}

public class CardService : ICardService
{
    public string PrintCardModelProperties(CardModel model)
    {
        return $"Card Number: {model.CardNumber}\n" +
               $"Name on Card: {model.NameOnCard}\n" +
               $"Cvv: {model.Cvv}\n" +
               $"Expiration Date: {model.ExpirationDate}\n";
    }
}