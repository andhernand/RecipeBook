using Microsoft.AspNetCore.Mvc;

using RecipeBook.ApiService.Filters;
using RecipeBook.ApiService.Mapping;
using RecipeBook.ApiService.Services;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class CreateRecipeEndpoint
{
    private const string Name = "CreateRecipe";

    public static void MapCreateRecipe(this IEndpointRouteBuilder builder)
    {
        builder.MapPost(ApiEndpoints.Recipes.Create, async (
                CreateRecipeRequest request,
                IRecipeService service,
                CancellationToken token) =>
            {
                var recipe = request.MapToRecipe();
                var created = await service.CreateRecipeAsync(recipe, token);
                var response = created.MapToResponse();

                return TypedResults.CreatedAtRoute(
                    response,
                    GetRecipeByIdEndpoint.Name,
                    new { id = created.Id });
            })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .Accepts<CreateRecipeRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<RequestValidationFilter<CreateRecipeRequest>>()
            .Produces<RecipeResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json");
    }
}