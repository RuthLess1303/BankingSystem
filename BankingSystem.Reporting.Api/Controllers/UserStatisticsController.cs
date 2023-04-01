using BankingSystem.Reporting.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Reporting.Api.Controllers;

[ApiController]
[Route("api/user-statistics")]
public class UserStatisticsController : ControllerBase
{
    private readonly IUserStatisticsRepository _userStatisticsRepository;

    public UserStatisticsController(IUserStatisticsRepository userStatisticsRepository)
    {
        _userStatisticsRepository = userStatisticsRepository;
    }

    [HttpGet("total-users-registered-in-current-year")]
    public string TotalUsersRegisteredCurrentYear()
    {
        var users = _userStatisticsRepository.TotalRegisteredUsersCurrentYear();

        return $"Total Users Registered In This Year: {users}";
    }
    
    [HttpGet("total-users-registered-in-last-year")]
    public string TotalUsersRegisteredLastYear()
    {
        var users = _userStatisticsRepository.TotalRegisteredUsersForLastYear();

        return $"Total Users Registered Last Year: {users}";
    }
    
    [HttpGet("total-users-registered-in-last-30-days")]
    public string TotalUsersRegisteredLast30Days()
    {
        var users = _userStatisticsRepository.TotalRegisteredUsersForLast30Days();

        return $"Total Users Registered In Last 30 Days: {users}";
    }
}