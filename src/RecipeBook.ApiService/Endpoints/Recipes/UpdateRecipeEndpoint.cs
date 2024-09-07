using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.ApiService.Filters;
using RecipeBook.ApiService.Mapping;
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
                    var recipe = request.MapToRecipe();
                    recipe.Id = id;

                    var updated = await service.UpdateRecipeAsync(id, recipe, token);
                    if (updated is null)
                    {
                        return TypedResults.NotFound();
                    }

                    var response = updated.MapToResponse();
                    return TypedResults.Ok(response);
                })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .Accepts<UpdateRecipeRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<RequestValidationFilter<UpdateRecipeRequest>>()
            .WithOpenApi();
    }
}