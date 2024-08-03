using Microsoft.AspNetCore.Mvc;

using RecipeBook.ApiService.Filters;
using RecipeBook.ApiService.Services;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class DeleteRecipeEndpoint
{
    private const string Name = "DeleteRecipe";

    public static void MapDeleteRecipe(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.Recipes.Delete, async (
                string id,
                IRecipeService service,
                CancellationToken token) =>
            {
                var deleted = await service.DeleteRecipeAsync(id, token);
                return deleted is null
                    ? Results.Problem(statusCode: StatusCodes.Status404NotFound)
                    : TypedResults.NoContent();
            })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .AddEndpointFilter<ObjectIdFilter>()
            .AddEndpointFilter<RecipeExistsFilter>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json");
    }
}