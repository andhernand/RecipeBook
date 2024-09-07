using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.ApiService.Services;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class GetRecipeByIdEndpoint
{
    public const string Name = "GetRecipeById";

    public static void MapGetRecipeById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Recipes.GetById,
                async Task<Results<Ok<RecipeResponse>, NotFound>> (
                    string id,
                    IRecipeService service,
                    CancellationToken token) =>
                {
                    var recipe = await service.GetRecipeByIdAsync(id, token);

                    return recipe is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(recipe);
                })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .WithOpenApi();
    }
}