using BankingSystemSharedDb.Db.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ReportingApi.Controllers;

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
    public async Task<string> TotalUsersRegisteredCurrentYear()
    {
        var users = await _userStatisticsRepository.TotalRegisteredUsersCurrentYear();

        return $"Total Users Registered In This Year: {users}";
    }
    
    [HttpGet("total-users-registered-in-last-year")]
    public async Task<string> TotalUsersRegisteredLastYear()
    {
        var users = await _userStatisticsRepository.TotalRegisteredUsersForLastYear();

        return $"Total Users Registered Last Year: {users}";
    }
    
    [HttpGet("total-users-registered-in-last-30-days")]
    public async Task<string> TotalUsersRegisteredLast30Days()
    {
        var users = await _userStatisticsRepository.TotalRegisteredUsersForLast30Days();

        return $"Total Users Registered In Last 30 Days: {users}";
    }
}