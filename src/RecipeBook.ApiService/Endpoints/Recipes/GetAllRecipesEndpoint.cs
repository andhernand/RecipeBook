using RecipeBook.ApiService.Mapping;
using RecipeBook.ApiService.Services;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class GetAllRecipesEndpoint
{
    private const string Name = "GetAllRecipes";

    public static void MapGetAllRecipes(this IEndpointRouteBuilder builder)
    {
        builder.MapGet(ApiEndpoints.Recipes.GetAll, async (
                IRecipeService service,
                CancellationToken token) =>
            {
                var recipes = await service.GetAllRecipesAsync(token);
                var response = recipes.MapToResponse();
                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .Produces<RecipesResponse>(contentType: "application/json");
    }
}