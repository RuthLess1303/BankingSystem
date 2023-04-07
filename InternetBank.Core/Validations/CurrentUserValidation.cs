using InternetBank.Db.Db.Repositories;

namespace InternetBank.Core.Validations;

public interface ICurrentUserValidation
{
    Task IsSameUserWithId(int userId);
    Task IsSameUserWithIban(string iban);
    Task IsSameUserWithPrivateNumber(string privateNumber);
    Task<string> GetLoggedUserPrivateNumber();
    
}

public class CurrentUserValidation : ICurrentUserValidation
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginLoggerRepository _loginLoggerRepository;
    private readonly IAccountValidation _accountValidation;

    public CurrentUserValidation(IUserRepository userRepository, 
        ILoginLoggerRepository loginLoggerRepository, 
        IAccountValidation accountValidation)
    {
        _userRepository = userRepository;
        _loginLoggerRepository = loginLoggerRepository;
        _accountValidation = accountValidation;
    }

    public async Task IsSameUserWithId(int userId)
    {
        var currentUser = await _loginLoggerRepository.GetLoggedUser();

        if (currentUser.UserId != userId)
        {
            throw new Exception("Please provide your ID");
        }
    }

    public async Task IsSameUserWithIban(string iban)
    {
        var currentUser = await _loginLoggerRepository.GetLoggedUser();
        var initiatorUser = await _userRepository.GetUserWithIban(iban);
            
        if (currentUser.UserId != initiatorUser.Id)
        {
            throw new Exception("Please provide your IBAN");
        }
    }
    
    public async Task IsSameUserWithPrivateNumber(string privateNumber)
    {
        var currentUser = await _loginLoggerRepository.GetLoggedUser();
        
        var initiatorUser = await _userRepository.FindWithPrivateNumber(privateNumber);
            
        if (initiatorUser == null || currentUser.UserId != initiatorUser.Id)
        {
            throw new Exception("Please provide your Private Number");
        }
    }

    public async Task<string> GetLoggedUserPrivateNumber()
    {
        var currentUser = await _loginLoggerRepository.GetLoggedUser();
        var userInfo = await _userRepository.FindWithId(currentUser.UserId);
        if (userInfo == null)
        {
            throw new Exception("User not found with provided Id");
        }

        return userInfo.PrivateNumber;
    }
}
