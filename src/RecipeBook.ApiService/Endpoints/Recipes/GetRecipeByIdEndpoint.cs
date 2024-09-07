using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.ApiService.Mapping;
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
                    if (recipe is null)
                    {
                        return TypedResults.NotFound();
                    }

                    var response = recipe.MapToResponse();
                    return TypedResults.Ok(response);
                })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .WithOpenApi();
    }
}