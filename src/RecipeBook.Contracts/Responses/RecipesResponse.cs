namespace RecipeBook.Contracts.Responses;

public record RecipesResponse
{
    public required IEnumerable<RecipeResponse> Recipes { get; init; } = [];
}