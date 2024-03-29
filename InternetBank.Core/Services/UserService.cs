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

    public async Task<UserEntity> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!checkPassword)
        {
            throw new Exception($"Incorrect credentials");
        }

        return user;
    }
    
    private async Task UserDataCheck(RegisterUserRequest request)
    {
        _propertyValidations.CheckNameOrSurname(request.Name);
        _propertyValidations.CheckNameOrSurname(request.Surname);
        _propertyValidations.CheckPrivateNumberFormat(request.PrivateNumber);
        var privateNumberUsage = await _propertyValidations.CheckPrivateNumberUsage(request.PrivateNumber);
        if (privateNumberUsage)
        {
            throw new Exception($"User already registered with provided Private Number: {request.PrivateNumber}");
        }
        _propertyValidations.CheckEmailDomainExistence(request.Email);
        await _propertyValidations.EmailInUse(request.Email);
        _propertyValidations.CheckStrongPassword(request.Password);
        _propertyValidations.IsOver18(request.BirthDate);
    }
}