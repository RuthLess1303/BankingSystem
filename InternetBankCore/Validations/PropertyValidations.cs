using System.Text;
using System.Text.RegularExpressions;
using InternetBankCore.Db.Models;
using InternetBankCore.Db.Repositories;

namespace InternetBankCore.Validations;

public interface IPropertyValidations
{
    bool CheckAmount(decimal amount);
    Task CheckCurrency(string currencyCode);
    void CheckIbanFormat(string iban);
    void CheckCardNumberFormat(string cardNumber);
    bool IsCardExpired(DateTime expirationDate);
    void CheckNameOnCard(string nameOnCard);
    void CheckEmailDomainExistence(string email);
    void CheckStrongPassword(string password);
    Task CheckPrivateNumberUsage(string privateNumber);
    void CheckNameOrSurname(string str);
    Task EmailInUse(string email);
    Task CheckIbanUsage(string iban);
}

public class PropertyValidations : IPropertyValidations
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICurrencyRepository _currencyRepository;
    private readonly IUserRepository _userRepository;

    public PropertyValidations(
        ICurrencyRepository currencyRepository,
        IUserRepository userRepository,
        IAccountRepository accountRepository)
    {
        _currencyRepository = currencyRepository;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
    }

    public bool CheckAmount(decimal amount)
    {
        return amount >= 0;
    }

    public async Task CheckCurrency(string currencyCode)
    {
        var currency = await _currencyRepository.FindCurrency(currencyCode);

        if (currency == null) throw new Exception($"Could not find currency code: {currencyCode}");
    }

    public void CheckIbanFormat(string iban)
    {
        iban = iban?.ToUpper().Replace(" ", "").Replace("-", "") ??
               throw new InvalidOperationException("Iban is null!");

        if (string.IsNullOrEmpty(iban)) throw new Exception("Iban can not be empty or null");

        if (iban.Length is < 15 or > 36) throw new Exception("Iban length should be between 15 and 36");

        if (!Regex.IsMatch(iban, "^[A-Z0-9]"))
            throw new Exception("Iban should contain only uppercase letters and numbers");

        var checkDigits = iban.Substring(0, 2);
        iban = string.Concat(iban.AsSpan(2), checkDigits);

        var sb = new StringBuilder();
        foreach (var c in iban)
            if (char.IsLetter(c))
                sb.Append(c - 'A' + 10);
            else
                sb.Append(c);

        var digits = sb.ToString();
        var mod = int.Parse(digits[..1]);
        Parallel.For(1, digits.Length, ctr =>
        {
            var digit = int.Parse(digits.Substring(ctr, 1));
            mod = (mod * 10 + digit) % 97;
        });

        if (mod != 1) throw new Exception("Iban has incorrect format");
    }

    public void CheckCardNumberFormat(string cardNumber)
    {
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

        if (sum % 10 != 0) throw new Exception("CardNumber is invalid");
    }

    public bool IsCardExpired(DateTime expirationDate)
    {
        if (expirationDate == DateTime.Now || expirationDate > DateTime.Now) return true;

        return false;
    }

    public void CheckNameOnCard(string nameOnCard)
    {
        if (!Regex.IsMatch(nameOnCard, @"^[a-zA-Z]+$"))
            throw new Exception("Name on card must contain only alphabetical characters");
    }

    public void CheckEmailDomainExistence(string email)
    {
        var domainIndex = email.IndexOf('@');
        var domainLastIndex = email.IndexOf('.');
        var domain = email.Substring(domainIndex, domainLastIndex - domainIndex);
        if (!Enum.IsDefined(typeof(EmailModel), domain)) throw new Exception("Incorrect email provider");
    }

    public void CheckStrongPassword(string password)
    {
        if (!Regex.IsMatch(password, @"^[a-z]"))
            throw new Exception("Password must contain minimum 1 lowercase character");
        if (!Regex.IsMatch(password, @"^[A-Z]"))
            throw new Exception("Password must contain minimum 1 uppercase character");
        if (!Regex.IsMatch(password, @"^[0-9]"))
            throw new Exception("Password must contain minimum 1 number");
        if (!Regex.IsMatch(password, @"\W\S")) throw new Exception("Password must contain minimum 1 symbol");
    }

    public async Task CheckPrivateNumberUsage(string privateNumber)
    {
        var user = await _userRepository.FindWithPrivateNumber(privateNumber);
        if (user != null) throw new Exception($"User already registered with provided Private Number: {privateNumber}");
    }

    public void CheckNameOrSurname(string str)
    {
        if (!Regex.IsMatch(str, @"^[a-zA-Z]+$")) throw new Exception("Please specify only alphabetical characters");
    }

    public async Task EmailInUse(string email)
    {
        var user = await _userRepository.FindWithEmail(email);
        if (user != null) throw new Exception("Email in use");
    }

    public async Task CheckIbanUsage(string iban)
    {
        var account = await _accountRepository.GetAccountWithIban(iban);
        if (account != null) throw new Exception("Iban already in use");
    }
}