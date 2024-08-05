using System.Net.Http.Json;

using Bogus;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;
// ReSharper disable MemberCanBePrivate.Global

namespace RecipeBook.ApiService.Tests;

public static class Mother
{
    public const string RecipesApiBasePath = "/api/recipes";

    public static CreateRecipeRequest GenerateCreateRecipeRequest(
        string? title = default,
        string? description = default,
        IEnumerable<string>? ingredients = default,
        IEnumerable<string>? directions = default)
    {
        var recipeFaker = new Faker<CreateRecipeRequest>()
            .RuleFor(x => x.Title, f => title ?? f.Lorem.Sentence(3))
            .RuleFor(x => x.Description, f => description ?? f.Lorem.Sentence(10))
            .RuleFor(x => x.Ingredients, f => ingredients ?? f.Make(6, () => f.Lorem.Sentence(3)))
            .RuleFor(x => x.Directions, f => directions ?? f.Make(10, () => f.Lorem.Sentence(10)));

        return recipeFaker.Generate();
    }

    public static UpdateRecipeRequest GenerateUpdateRecipeRequest(
        string? title = default,
        string? description = default,
        IEnumerable<string>? ingredients = default,
        IEnumerable<string>? directions = default)
    {
        var recipeFaker = new Faker<UpdateRecipeRequest>()
            .RuleFor(x => x.Title, f => title ?? f.Lorem.Sentence(3))
            .RuleFor(x => x.Description, f => description ?? f.Lorem.Sentence(10))
            .RuleFor(x => x.Ingredients, f => ingredients ?? f.Make(6, () => f.Lorem.Sentence(3)))
            .RuleFor(x => x.Directions, f => directions ?? f.Make(10, () => f.Lorem.Sentence(10)));

        return recipeFaker.Generate();
    }

    public static ProblemDetails GenerateNotFoundProblemDetails()
    {
        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            Title = "Not Found",
            Status = StatusCodes.Status404NotFound
        };
    }

    public static ValidationProblemDetails GenerateValidationProblemDetails(Dictionary<string, string[]> errors)
    {
        return new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest
        };
    }

    public static async Task<RecipeResponse> CreateRecipeAsync(HttpClient client)
    {
        var request = GenerateCreateRecipeRequest();
        var response = await client.PostAsJsonAsync(RecipesApiBasePath, request);
        var recipe = await response.Content.ReadFromJsonAsync<RecipeResponse>();
        return recipe!;
    }
}