using RecipeBook.ApiService.Repositories;

namespace RecipeBook.ApiService.Filters;

public class RecipeExistsFilter : IEndpointFilter
{
    private readonly IRecipeRepository _repository;
    private readonly ILogger<RecipeExistsFilter> _logger;

    public RecipeExistsFilter(IRecipeRepository repository, ILogger<RecipeExistsFilter> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var recipeId = context.Arguments.OfType<string>().First();
        var exists = await _repository.ExistsById(recipeId, context.HttpContext.RequestAborted);
        if (!exists)
        {
            var problem = Results.Problem(statusCode: StatusCodes.Status404NotFound);
            _logger.LogWarning("A Recipe with {RecipeId} does not exist in the system", recipeId);
            return problem;
        }

        return await next(context);
    }
}