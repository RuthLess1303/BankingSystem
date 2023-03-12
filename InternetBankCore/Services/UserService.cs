using System.Security.Cryptography;
using System.Text;
using InternetBankCore.Db.Entities;
using InternetBankCore.Db.Repositories;
using InternetBankCore.Requests;
using InternetBankCore.Validations;

namespace InternetBankCore.Services;

public interface IUserService
{
    Task Register(RegisterUserRequest request);
    Task CreateAccount(CreateAccountRequest request);
    Task CreateCard(CreateCardRequest request);
    Task Login(LoginRequest request);
}

public class UserService : IUserService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IPropertyValidations _propertyValidations;
    private readonly IUserRepository _userRepository;

    public UserService(
        IPropertyValidations propertyValidations,
        IUserRepository userRepository,
        IAccountRepository accountRepository)
    {
        _propertyValidations = propertyValidations;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
    }

    public async Task Register(RegisterUserRequest request)
    {
        await UserDataCheck(request);
        await _userRepository.Register(request);
    }

    public async Task CreateAccount(CreateAccountRequest request)
    {
        await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
        _propertyValidations.CheckIbanFormat(request.Iban);
        await _propertyValidations.CheckIbanUsage(request.Iban);
        await _propertyValidations.CheckCurrency(request.CurrencyCode);
        var forHash = request.Iban + request.Amount + DateTime.Now;

        var accountEntity = new AccountEntity
        {
            Id = Guid.NewGuid(),
            PrivateNumber = request.PrivateNumber,
            Iban = request.Iban,
            CurrencyCode = request.CurrencyCode,
            Amount = request.Amount,
            Hash = GetHash(forHash),
            CreationDate = DateTime.Now
        };

        await _accountRepository.Create(accountEntity);
    }

    public async Task CreateCard(CreateCardRequest request)
    {
        if (request.ExpirationDate <= DateTime.Now || request.ExpirationDate.Year <= DateTime.Now.Year)
            throw new Exception("Expiration date must be more than 1 year apart");

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

        await _userRepository.CreateCard(cardEntity);
    }

    public async Task Login(LoginRequest request)
    {
        var user = await _userRepository.GetUserWithEmail(request.Email);
        var operatorEntity = await _userRepository.GetOperatorWithEmail(request.Email);

        if (user == null || user.Password != request.Password)
            throw new Exception("Incorrect credentials");
        if (operatorEntity == null || operatorEntity.Password != request.Password)
            throw new Exception("Incorrect credentials");
    }

    private async Task UserDataCheck(RegisterUserRequest request)
    {
        _propertyValidations.CheckStrongPassword(request.Password);
        await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
        _propertyValidations.CheckNameOrSurname(request.Name);
        _propertyValidations.CheckNameOrSurname(request.Surname);
        _propertyValidations.CheckEmailDomainExistence(request.Email);
        await _propertyValidations.EmailInUse(request.Email);
    }

    private string GetHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var builder = new StringBuilder();
        foreach (var t in bytes)
            builder.Append(t.ToString("x2"));

        return builder.ToString();
    }
}