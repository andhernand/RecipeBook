using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.ApiService.Filters;
using RecipeBook.ApiService.Services;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class UpdateRecipeEndpoint
{
    private const string Name = "UpdateRecipe";

    public static void MapUpdateRecipe(this IEndpointRouteBuilder builder)
    {
        builder.MapPut(ApiEndpoints.Recipes.Update,
                async Task<Results<Ok<RecipeResponse>, NotFound, ValidationProblem>> (
                    string id,
                    UpdateRecipeRequest request,
                    IRecipeService service,
                    CancellationToken token) =>
                {
                    var updated = await service.UpdateAsync(id, request, token);

                    return updated is null
                        ? TypedResults.NotFound()
                        : TypedResults.Ok(updated);
                })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .Accepts<UpdateRecipeRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<RequestValidationFilter<UpdateRecipeRequest>>()
            .WithOpenApi();
    }
}