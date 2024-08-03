using RecipeBook.ApiService.Validators;

namespace RecipeBook.ApiService.Filters;

public class ObjectIdFilter : IEndpointFilter
{
    private readonly ObjectIdValidator _validator;
    private readonly ILogger<ObjectIdFilter> _logger;

    public ObjectIdFilter(ObjectIdValidator validator, ILogger<ObjectIdFilter> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var objectId = context.Arguments.OfType<string>().First();
        var result = await _validator.ValidateAsync(objectId, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            var problem = TypedResults.ValidationProblem(result.ToDictionary());
            _logger.LogWarning("The {ObjectId} is not valid", objectId);
            return problem;
        }

        return await next(context);
    }
}