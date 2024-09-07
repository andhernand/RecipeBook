using RecipeBook.ApiService.Models;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Mapping;

public static class ContractMapping
{
    public static Recipe MapToRecipe(this CreateRecipeRequest request)
    {
        return new Recipe
        {
            Title = request.Title,
            Description = request.Description,
            Ingredients = request.Ingredients,
            Directions = request.Directions
        };
    }

    public static Recipe MapToRecipe(this UpdateRecipeRequest request)
    {
        return new Recipe
        {
            Id = request.Id,
            Title = request.Title,
            Description = request.Description,
            Ingredients = request.Ingredients,
            Directions = request.Directions
        };
    }

    public static RecipeResponse MapToResponse(this Recipe recipe)
    {
        return new RecipeResponse
        {
            Id = recipe.Id,
            Title = recipe.Title,
            Description = recipe.Description,
            Ingredients = recipe.Ingredients,
            Directions = recipe.Directions
        };
    }

    public static IEnumerable<RecipeResponse> MapToResponse(this IEnumerable<Recipe> recipes)
    {
        return recipes.Select(MapToResponse);
    }
}