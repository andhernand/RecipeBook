using Microsoft.AspNetCore.Http.HttpResults;

using RecipeBook.ApiService.Services;

namespace RecipeBook.ApiService.Endpoints.Recipes;

public static class DeleteRecipeEndpoint
{
    private const string Name = "DeleteRecipe";

    public static void MapDeleteRecipe(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete(ApiEndpoints.Recipes.Delete,
                async Task<Results<NoContent, NotFound>> (
                    string id,
                    IRecipeService service,
                    CancellationToken token) =>
                {
                    var deleted = await service.DeleteRecipeAsync(id, token);
                    return deleted is null
                        ? TypedResults.NotFound()
                        : TypedResults.NoContent();
                })
            .WithName(Name)
            .WithTags(ApiEndpoints.Recipes.Tag)
            .WithOpenApi();
    }
}