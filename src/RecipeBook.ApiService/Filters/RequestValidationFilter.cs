using FluentValidation;

namespace RecipeBook.ApiService.Filters;

public class RequestValidationFilter<T> : IEndpointFilter
{
    private readonly IValidator<T> _validator;
    private readonly ILogger<RequestValidationFilter<T>> _logger;

    public RequestValidationFilter(IValidator<T> validator, ILogger<RequestValidationFilter<T>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().First();
        var result = await _validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            var problems = TypedResults.ValidationProblem(result.ToDictionary());
            _logger.LogWarning("The {Request} had the following {@Problems}", typeof(T).Name, problems);
            return problems;
        }

        return await next(context);
    }
}