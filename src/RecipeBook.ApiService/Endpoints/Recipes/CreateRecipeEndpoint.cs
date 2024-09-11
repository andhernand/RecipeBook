using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.ApiService.Filters;
using RecipeBook.ApiService.Services;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class CreateRecipeEndpoint
{
    private const string Name = "CreateRecipe";

    public static void MapCreateRecipe(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Recipes.Create,
                async Task<Results<CreatedAtRoute<RecipeResponse>, ValidationProblem>> (
                    CreateRecipeRequest request,
                    IRecipeService service,
                    CancellationToken token) =>
                {
                    var recipe = await service.CreateAsync(request, token);

                    return TypedResults.CreatedAtRoute(
                        recipe,
                        GetRecipeByIdEndpoint.Name,
                        new { id = recipe.Id });
                })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .Accepts<CreateRecipeRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<RequestValidationFilter<CreateRecipeRequest>>()
            .WithOpenApi();
    }
}