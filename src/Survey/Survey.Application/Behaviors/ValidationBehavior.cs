using FluentValidation;
using MediatR;

namespace Survey.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(res => res.Errors).Where(error => error is not null).ToList();

                if (failures.Count != 0)
                {
                    var message = failures.Select(error => $"{error.PropertyName} : {error.ErrorMessage}").FirstOrDefault();

                    throw new System.ComponentModel.DataAnnotations.ValidationException(message);
                }
            }
            return await next();
        }
    }
}
