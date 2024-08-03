namespace RecipeBook.Contracts.Requests;

public class UpdateRecipeRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IEnumerable<string> Ingredients { get; init; } = [];
    public required IEnumerable<string> Directions { get; init; } = [];
}