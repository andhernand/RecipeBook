namespace RecipeBook.Contracts.Responses;

public record RecipeResponse
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IEnumerable<string> Ingredients { get; init; } = [];
    public required IEnumerable<string> Directions { get; init; } = [];
}