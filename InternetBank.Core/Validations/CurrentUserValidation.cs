using InternetBank.Db.Db.Repositories;

namespace InternetBank.Core.Validations;

public interface ICurrentUserValidation
{
    Task IsSameUserWithId(int userId);
    Task IsSameUserWithIban(string iban);
    Task IsSameUserWithPrivateNumber(string privateNumber);
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
            
        if (currentUser.UserId != initiatorUser.Id)
        {
            throw new Exception("Please provide your Private Number");
        }
    }
}