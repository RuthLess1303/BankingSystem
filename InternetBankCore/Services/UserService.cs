using System.Security.Cryptography;
using System.Text;
using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;
using InternetBankCore.Validations;
using Microsoft.AspNetCore.Identity;

namespace InternetBankCore.Services;

public interface IUserService
{
    Task Register(RegisterUserRequest request);
    Task CreateAccount(CreateAccountRequest request);
    Task CreateCard(CreateCardRequest request);
    Task<UserEntity> Login(LoginRequest request);
}

public class UserService : IUserService
{
    private readonly IPropertyValidations _propertyValidations;
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly UserManager<UserEntity> _userManager;

    public UserService(
        IPropertyValidations propertyValidations, 
        IUserRepository userRepository, 
        IAccountRepository accountRepository,
        UserManager<UserEntity> userManager)
    {
        _propertyValidations = propertyValidations;
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _userManager = userManager;
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
        var forHash = request.Iban + request.Amount.ToString() + DateTime.Now.ToString();
        
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

    private string GetHash(string input)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public async Task CreateCard(CreateCardRequest request)
    {
        if (request.ExpirationDate <= DateTime.Now || request.ExpirationDate.Year <= DateTime.Now.Year)
        {
            throw new Exception("Expiration date must be more than 1 year apart");
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

        await _userRepository.CreateCard(cardEntity);
    }

    public async Task<UserEntity> Login(LoginRequest request)
    {
        // var user = await _userManager.FindByEmailAsync(request.Email);
        var user = await _userRepository.FindWithEmail(request.Email);
        
        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        if (user == null || !checkPassword)
        {
            Console.WriteLine(checkPassword);
            throw new Exception($"Incorrect credentials: {await _userManager.CheckPasswordAsync(user, request.Password)}");
        }

        return user;
    }
}