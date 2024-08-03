using Microsoft.AspNetCore.Mvc;

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
        builder.MapPut(ApiEndpoints.Recipes.Update, async (
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
                    return Results.Problem(statusCode: StatusCodes.Status404NotFound);
                }

                var response = updated.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .Accepts<UpdateRecipeRequest>(isOptional: false, contentType: "application/json")
            .AddEndpointFilter<ObjectIdFilter>()
            .AddEndpointFilter<RecipeExistsFilter>()
            .AddEndpointFilter<RequestValidationFilter<UpdateRecipeRequest>>()
            .Produces<RecipeResponse>(contentType: "application/json")
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound, contentType: "application/problem+json")
            .Produces<ValidationProblemDetails>(
                StatusCodes.Status400BadRequest,
                contentType: "application/problem+json");
    }
}