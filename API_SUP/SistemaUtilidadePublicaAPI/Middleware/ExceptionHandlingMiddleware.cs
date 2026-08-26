using SistemaUtilidadePublicaAPI.Common.Exceptions;
//using SistemaUtilidadePublicaAPI.Common.Exceptions;
using SistemaUtilidadePublicaAPI.Data.Repositories;
using System.Net;
using System.Text.Json;

namespace SistemaUtilidadePublicaAPI.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EmailAlreadyExistsException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (EmailNotExistsException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (InvalidPasswordException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (ExceptionCommon ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            context.Response.ContentType = "application/json";

            var response = new
            {
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType =
                "application/json";

            var response = new
            {
                message = "Ocorreu um erro interno no servidor."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}