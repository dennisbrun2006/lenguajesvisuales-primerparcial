namespace BibliotecaApi.Middlewares;

using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    public ErrorHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            var resp = new
            {
                success = false,
                error = new
                {
                    code = "SERVER_ERROR",
                    message = ex.Message
                }
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(resp));
        }
    }
}
