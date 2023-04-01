using System.Text;
using Newtonsoft.Json;

namespace BankingSystem.Reporting.Api.Middlewares;

public class GlobalErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            var error = new { message = ex.Message };
            var errorJson = JsonConvert.SerializeObject(error);
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(errorJson, Encoding.UTF8);
        }
    }
}