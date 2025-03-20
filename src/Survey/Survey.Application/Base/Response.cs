using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Survey.Application.Base
{
    public class Response<T>
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
        public object? Meta { get; set; }
        public bool Succeeded { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }

        public Dictionary<string, object?> ToNonNullDictionary()
        {
            return new Dictionary<string, object?>
            {
                { nameof(StatusCode), StatusCode },
                { nameof(Meta), Meta },
                { nameof(Succeeded), Succeeded },
                { nameof(Message), Message },
                { nameof(Errors), Errors?.Count > 0 ? Errors : null },
                { nameof(Data), Data }
            }
            .Where(pair => pair.Value is not null) // Remove entries with null values
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public ObjectResult ToObjectResult()
        {
            Dictionary<string, object?> response = ToNonNullDictionary();

            return StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(response),
                HttpStatusCode.Created => new CreatedResult(string.Empty, response),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(response),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(response),
                HttpStatusCode.NotFound => new NotFoundObjectResult(response),
                HttpStatusCode.Accepted => new AcceptedResult(string.Empty, response),
                HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(response),
                HttpStatusCode.Forbidden => new ObjectResult(response) { StatusCode = 403 },
                _ => new BadRequestObjectResult(response)
            };
            //return new ObjectResult(ToNonNullDictionary())
            //{
            //    StatusCode = (int)StatusCode
            //};
        }
    }
}
