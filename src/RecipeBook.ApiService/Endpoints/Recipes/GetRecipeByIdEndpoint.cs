using Microsoft.AspNetCore.Mvc;

using RecipeBook.ApiService.Filters;
using RecipeBook.ApiService.Mapping;
using RecipeBook.ApiService.Services;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class GetRecipeByIdEndpoint
{
    public const string Name = "GetRecipeById";

    public static void MapGetRecipeById(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Recipes.GetById, async (
                string id,
                IRecipeService service,
                CancellationToken token) =>
            {
                var recipe = await service.GetRecipeByIdAsync(id, token);
                if (recipe is null)
                {
                    return Results.Problem(statusCode: StatusCodes.Status404NotFound);
                }

                var response = recipe.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .AddEndpointFilter<ObjectIdFilter>()
            .Produces<RecipeResponse>(contentType: "application/json")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json");
    }
}