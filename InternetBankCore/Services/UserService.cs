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