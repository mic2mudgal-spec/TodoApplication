using System.Net;
using System.Text.Json;
using Todo.Api.Models;
using Todo.Application.Exceptions;

namespace Todo.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteResponse(
                context,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (NotFoundException ex)
        {
            await WriteResponse(
                context,
                HttpStatusCode.NotFound,
                ex.Message);
        }
        catch (Exception)
        {
            await WriteResponse(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            Message = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
