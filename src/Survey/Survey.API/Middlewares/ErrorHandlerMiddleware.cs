using Microsoft.EntityFrameworkCore;
using Survey.Application.Base;
using Survey.Domain.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Survey.API.Middlewares
{
    public class ErrorHandlerMiddleware (ILogger<ErrorHandlerMiddleware> logger, RequestDelegate next) 
    {
        private readonly ILogger<ErrorHandlerMiddleware> _logger = logger ;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                // Ensure error responses are thrown
                if (!context.Response.HasStarted)
                {
                    HandleAutomaticStatusCodeErrors(context);
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Error occurred in request: {@Request}", error.Message);
                await HandleExceptionAsync(context, error);
            }
        }

        private void HandleAutomaticStatusCodeErrors(HttpContext context)
        {
            var statusCode = context.Response.StatusCode;
            if (statusCode is 404 or 401 or 403)
            {
                throw statusCode switch
                {
                    404 => new KeyNotFoundException("Not Found!"),
                    401 => new UnauthorizedAccessException("Not Authorized!"),
                    403 => new AccessViolationException("Forbidden!"),
                    _ => new Exception("Unexpected error")
                };
            }
        }

        private  async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var responseModel = new Response<string>
            {
                Succeeded = false,
                Message = error.Message
            };

            response.StatusCode = error switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ValidationException  => (int)HttpStatusCode.UnprocessableEntity,
                DomainException => (int)HttpStatusCode.UnprocessableEntity,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                DbUpdateException => (int)HttpStatusCode.BadRequest,
                AccessViolationException => (int)HttpStatusCode.Forbidden,
                _ => (int)HttpStatusCode.InternalServerError
            };

            responseModel.StatusCode = (HttpStatusCode)response.StatusCode;

            // Include inner exception details if available
            if (error.InnerException != null)
                responseModel.Message += $"\nInner Exception: {error.InnerException.Message}";

            var result = JsonSerializer.Serialize(responseModel.ToNonNullDictionary());
            await response.WriteAsync(result);
        }
    }
}
