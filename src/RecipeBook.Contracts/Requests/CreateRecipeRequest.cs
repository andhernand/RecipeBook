﻿namespace RecipeBook.Contracts.Requests;

public record CreateRecipeRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required IEnumerable<string> Ingredients { get; init; } = [];
    public required IEnumerable<string> Directions { get; init; } = [];
}