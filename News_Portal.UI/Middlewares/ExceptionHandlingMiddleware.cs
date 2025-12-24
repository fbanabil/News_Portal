using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace News_Portal.UI.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException uex)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    var accept = context.Request.Headers["Accept"].ToString();
                    if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.ContentType = "application/json";
                        var payload = JsonSerializer.Serialize(new { error = "forbidden", message = uex.Message });
                        await context.Response.WriteAsync(payload);
                    }
                    else
                    {
                        context.Response.Redirect("/Home/AccessDenied");
                    }
                }
            }
            catch (Exception)
            {
                if (!context.Response.HasStarted)
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    var accept = context.Request.Headers["Accept"].ToString();
                    if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Response.ContentType = "application/json";
                        var payload = JsonSerializer.Serialize(new { error = "server_error", message = "An unexpected error occurred." });
                        await context.Response.WriteAsync(payload);
                    }
                    else
                    {
                        context.Response.Redirect("/Home/Error");
                    }
                }
            }
        }
    }
}