using System.Net;
using System.Text.Json;
using Gym.Helpers.Enums;
using Gym.Helpers.Exceptions;
using Gym.Helpers.Utils;
using Gym.Presentation.Controllers;
using Microsoft.AspNetCore.Components.Routing;

namespace Gym.Presentation.Middlewares;

public class GlobalExceptionHandling : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandling> _logger;

    public GlobalExceptionHandling(ILogger<GlobalExceptionHandling> logger)
        => _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (GlobalException e)
        {
            context.Response.StatusCode = (int)e.StatusCode;
            context.Response.ContentType = "application/json";

            var errorResponse = new GlobalHttpResponse((int)e.StatusCode, e.Message);
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);

            var errorResponse = new GlobalHttpResponse((int)HttpStatusCodes.InternalServerError,
                    "An internal server has occurred");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}
