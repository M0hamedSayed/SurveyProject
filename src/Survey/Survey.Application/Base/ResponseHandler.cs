using System.Net;
using Survey.Domain.Abstractions;

namespace Survey.Application.Base
{
    public class ResponseHandler
    {
        public Response<T> Create<T>(HttpStatusCode statusCode, string message, T? data = default, object? meta = null)
        {
            return new Response<T>
            {
                StatusCode = statusCode,
                Succeeded = statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created,
                Message = message,
                Data = data,
                Meta = meta
            };
        }

        public Response<T> Deleted<T>(string message = "Deleted Successfully") =>
            Create<T>(HttpStatusCode.OK, message);

        public Response<T> SuccessWithoutData<T>(string message = "Operation completed successfully", object? meta = null) =>
            Create<T>(HttpStatusCode.OK, message, default, meta);

        public Response<T> Success<T>(T data, string message = "Data retrieved successfully", object? meta = null) =>
            Create<T>(HttpStatusCode.OK, message, data, meta);
        public Response<T> Unauthorized<T>(string message = "Unauthorized access") =>
            Create<T>(HttpStatusCode.Unauthorized, message);

        public Response<T> BadRequest<T>(string message = "Invalid request") =>
            Create<T>(HttpStatusCode.BadRequest, message);

        public Response<T> UnprocessableEntity<T>(string message = "Validation or syntax errors") =>
            Create<T>(HttpStatusCode.UnprocessableEntity, message);

        public Response<T> NotFound<T>(string message = "Resource not found") =>
            Create<T>(HttpStatusCode.NotFound, message);

        public Response<T> Created<T>(T data, string message = "Created successfully", object? meta = null) =>
            Create<T>(HttpStatusCode.Created, message, data, meta);
    }
}
