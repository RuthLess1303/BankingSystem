using System.Text;
using InternetBank.Db.Db.Repositories;
using Newtonsoft.Json;

namespace InternetBank.Api.Middlewares;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GlobalErrorHandlingMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var scope = _serviceScopeFactory.CreateScope();
            var logger = scope.ServiceProvider.GetService<ILoggerRepository>();
            await logger!.AddLogInDb(ex, "Internet Bank");
            var error = new { message = ex.Message };
            var errorJson = JsonConvert.SerializeObject(error);
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(errorJson, Encoding.UTF8);
        }
    }
}