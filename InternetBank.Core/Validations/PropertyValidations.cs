using System.Text.RegularExpressions;
using IbanNet;
using InternetBank.Db.Db.Repositories;

namespace InternetBank.Core.Validations;

public interface IPropertyValidations
{
    bool IsAmountValid(decimal amount);
    Task CheckCurrency(string currencyCode);
    void CheckIbanFormat(string iban);
    Task CheckCardNumberFormat(string cardNumber);
    void CvvValidation(string cvv);
    bool IsCardExpired(DateTime expirationDate);
    void CheckNameOnCard(string nameOnCard);
    void CheckEmailDomainExistence(string email);
    void CheckStrongPassword(string password);
    Task<bool> CheckPrivateNumberUsage(string privateNumber);
    void CheckPrivateNumberFormat(string privateNumber);
    void CheckNameOrSurname(string str);
    Task EmailInUse(string email);
    Task CheckIbanUsage(string iban);
    void PinValidation(string pin);
}

public class PropertyValidations : IPropertyValidations
{
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICardRepository _cardRepository;

    public PropertyValidations(
        ICurrencyRepository currencyRepository, 
        IUserRepository userRepository, 
        IAccountRepository accountRepository, 
        ICardRepository cardRepository)
    {
        _currencyRepository = currencyRepository;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _cardRepository = cardRepository;
    }

    public bool IsAmountValid(decimal amount)
    {
        return amount >= 0;
    }

    public async Task CheckCurrency(string currencyCode)
    {
        if (currencyCode.ToUpper() == "GEL")
        {
            return;
        }
        var currency = await _currencyRepository.FindCurrency(currencyCode);
        
        if (currency == null)
        {
            throw new Exception($"Could not find currency code: {currencyCode}");
        }
    }
    
    public void CheckIbanFormat(string iban)
    {
        iban = iban?.ToUpper().Replace(" ", "").Replace("-", "") ?? throw new InvalidOperationException("Iban is null!");
        if (iban.Length < 15 || iban.Length > 36)
        {
            throw new Exception("Invalid IBAN length");
        }

        var countryCode = iban.Substring(0,2);
        var bankCode = iban.Substring(4,2);
        if (!Regex.IsMatch(countryCode, @"^[a-zA-Z]+$") || !Regex.IsMatch(bankCode, @"^[a-zA-Z]+$"))
        {
            throw new Exception("IBAN does not contain CountryCode or BankCode");
        }

        if (Regex.IsMatch(iban, @"^(?=.*[\W_]).+$"))
        {
            throw new Exception("IBAN should not contain Symbols");
        }
        var validator = new IbanValidator();
        var result = validator.Validate(iban);
        
        if (!result.IsValid)
        {
            Console.WriteLine("IBAN is not valid.");
        }
    }

    public void CvvValidation(string cvv)
    {
        if (cvv.Length != 3)
        {
            throw new Exception("CVV length must be 3");
        }

        if (Regex.IsMatch(cvv, @"[A-Za-z]") || Regex.IsMatch(cvv, @"^(?=.*[\W_]).+$"))
        {
            throw new Exception("CVV must contain only numbers");
        }
    }
    
    public void PinValidation(string pin)
    {
        if (pin.Length != 4)
        {
            throw new Exception("PIN length must be 4");
        }

        if (Regex.IsMatch(pin, @"[A-Za-z]") || Regex.IsMatch(pin, @"^(?=.*[\W_]).+$"))
        {
            throw new Exception("PIN must contain only numbers");
        }
    }
    
    public async Task CheckCardNumberFormat(string cardNumber)
    {
        if (cardNumber.Length != 16)
        {
            throw new Exception("Card number must be 16");
        }

        if (Regex.IsMatch(cardNumber, @"^(?=.*[a-zA-Z]).+$") || Regex.IsMatch(cardNumber, @"^(?=.*[\W_]).+$"))
        {
            throw new Exception("Card Number must contain only numbers");
        }
        await _cardRepository.CardNumberUsage(cardNumber);

        var sum = 0;
        Parallel.For(cardNumber.Length - 1, -1, i =>
        {
            var digit = int.Parse(cardNumber[i].ToString());
            if (i % 2 == 1)
            {
                digit *= 2;
                if (digit > 9) digit -= 9;
            }

            Interlocked.Add(ref sum, digit);
        });

        if (sum % 10 != 0)
        {
            throw new Exception("CardNumber is invalid");
        }
    }

    public bool IsCardExpired(DateTime expirationDate)
    {
        return expirationDate <= DateTime.Now;
    }

    public void CheckNameOnCard(string nameOnCard)
    {
        if(Regex.IsMatch(nameOnCard, "^(?=.*\\d)(?=.*[\\W_]).*$"))
        {
            throw new Exception("Name on card must contain only alphabetical characters");
        }
    }
    
    public void CheckEmailDomainExistence(string email)
    {
        var validEmail = new EmailValidation.EmailAttribute().IsValid(email);
        if (!validEmail)
        {
            throw new Exception("Incorrect email provider");
        }
    }

    public void CheckStrongPassword(string password)
    {
        if (!Regex.IsMatch(password, @"^(?=.*[a-z]).+$"))
        {
            throw new Exception("Password must contain minimum 1 lowercase character");
        }
        if (!Regex.IsMatch(password, @"^(?=.*[A-Z]).+$"))
        {
            throw new Exception("Password must contain minimum 1 uppercase character");
        }
        if (!Regex.IsMatch(password, @"^(?=.*[0-9]).+$"))
        {
            throw new Exception("Password must contain minimum 1 number");
        }
        if (!Regex.IsMatch(password, @"^(?=.*[\W_]).+$"))
        {
            throw new Exception("Password must contain minimum 1 symbol");
        }
        if (password.Length < 8)
        {
            throw new Exception("Password must have minimum lenght of 8");
        }
    }

    public async Task<bool> CheckPrivateNumberUsage(string privateNumber)
    {
        var user = await _userRepository.FindWithPrivateNumber(privateNumber);
        return user != null;
    }

    public void CheckPrivateNumberFormat(string privateNumber)
    {
        if (Regex.IsMatch(privateNumber, @"[A-Za-z]") || Regex.IsMatch(privateNumber, @"^(?=.*[\W_]).+$"))
        {
            throw new Exception("Private Number must contain only numbers");
        }

        if (!Regex.IsMatch(privateNumber, @"^(?=.*[0-9]).+$") || privateNumber.Length != 11)
        {
            throw new Exception("Invalid Private number format");
        }
    }
    
    public void CheckNameOrSurname(string str)
    {
        if(!Regex.IsMatch(str, @"^[a-zA-Z]+$"))
        {
            throw new Exception("Please specify only alphabetical characters");
        }
    }

    public async Task EmailInUse(string email)
    {
        var user = await _userRepository.FindWithEmail(email);
        if (user != null)
        {
            throw new Exception("Email in use");
        }
    }

    public async Task CheckIbanUsage(string iban)
    {
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account != null)
        {
            throw new Exception("Iban already in use");
        }
    }
}