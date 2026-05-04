using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinguaSwap.Api.Middleware
{
    public sealed class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteProblem(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteProblem(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await WriteProblem(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteProblem(context, HttpStatusCode.InternalServerError, "Unexpected server error");
            }
        }

        private static async Task WriteProblem(HttpContext context, HttpStatusCode code, string message)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)code;

            var problem = new ProblemDetails
            {
                Status = (int)code,
                Title = code.ToString(),
                Detail = message
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
