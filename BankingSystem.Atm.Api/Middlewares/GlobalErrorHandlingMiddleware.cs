using System.Text;
using InternetBank.Db.Db.Repositories;
using Newtonsoft.Json;

namespace BankingSystem.Atm.Api.Middlewares;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerRepository _loggerRepository;

    public GlobalErrorHandlingMiddleware(RequestDelegate next, ILoggerRepository loggerRepository)
    {
        _next = next;
        _loggerRepository = loggerRepository;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await _loggerRepository.AddLogInDb(ex, "Internet Bank");
            var error = new { message = ex.Message };
            var errorJson = JsonConvert.SerializeObject(error);
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(errorJson, Encoding.UTF8);
        }
    }
}