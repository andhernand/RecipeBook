namespace RecipeBook.Contracts.Responses;

public class RecipesResponse
{
    public required IEnumerable<RecipeResponse> Recipes { get; init; } = [];
}