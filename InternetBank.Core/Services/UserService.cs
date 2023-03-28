using System.Security.Cryptography;
using System.Text;
using InternetBank.Core.Validations;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;

namespace InternetBank.Core.Services;

public interface IUserService
{
    Task Register(RegisterUserRequest request);
    Task<UserEntity> Login(LoginRequest request);
}

public class UserService : IUserService
{
    private readonly IPropertyValidations _propertyValidations;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;

    public UserService(
        IPropertyValidations propertyValidations, 
        IUserRepository userRepository,
        UserManager<UserEntity> userManager)
    {
        _propertyValidations = propertyValidations;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task Register(RegisterUserRequest request)
    {
        await UserDataCheck(request);
        await _userRepository.Register(request);
    }

<<<<<<< HEAD:InternetBankCore/Services/UserService.cs
=======
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
            CardHolderName = request.NameOnCard,
            Cvv = request.Cvv,
            Pin = request.Pin,
            ExpirationDate = request.ExpirationDate,
            CreationDate = DateTime.Now
        };

        await _cardRepository.LinkWithAccount(request.Iban, cardEntity.Id);
        await _userRepository.CreateCard(cardEntity);
    }
    

>>>>>>> 8aea98499f6e1072c2bed6b80e900095f37d6c23:InternetBank.Core/Services/UserService.cs
    public async Task<UserEntity> Login(LoginRequest request)
    {
        var user = await _userRepository.FindWithEmail(request.Email);
        
        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (user == null || !checkPassword)
        {
            Console.WriteLine(checkPassword);
            throw new Exception($"Incorrect credentials: {await _userManager.CheckPasswordAsync(user, request.Password)}");
        }

        return user;
    }
    
    private async Task UserDataCheck(RegisterUserRequest request)
    {
        _propertyValidations.CheckStrongPassword(request.Password);
        _propertyValidations.CheckPrivateNumberFormat(request.PrivateNumber);
        var privateNumberUsage = await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
        if (privateNumberUsage)
        {
            throw new Exception($"User already registered with provided Private Number: {request.PrivateNumber}");
        }
        _propertyValidations.CheckNameOrSurname(request.Name);
        _propertyValidations.CheckNameOrSurname(request.Surname);
        _propertyValidations.CheckEmailDomainExistence(request.Email);
        await _propertyValidations.EmailInUse(request.Email);
    }
}