using InternetBank.Db.Db.Repositories;

namespace InternetBank.Core.Validations;

public interface ICurrentUserValidation
{
    Task IsSameUserWithIban(string iban);
    Task<string> GetLoggedUserPrivateNumber();
    
}

public class CurrentUserValidation : ICurrentUserValidation
{
    private readonly IUserRepository _userRepository;
    private readonly ILoginLoggerRepository _loginLoggerRepository;

    public CurrentUserValidation(IUserRepository userRepository, 
        ILoginLoggerRepository loginLoggerRepository)
    {
        _userRepository = userRepository;
        _loginLoggerRepository = loginLoggerRepository;
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
