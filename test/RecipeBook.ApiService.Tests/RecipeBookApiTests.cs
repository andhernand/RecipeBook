using System.Net.Http.Json;

using FluentAssertions;

using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Tests;

public class RecipeBookApiTests : IClassFixture<RecipeBookApiFactory>, IAsyncLifetime
{
    private readonly RecipeBookApiFactory _factory;
    private readonly List<string> _createdRecipeIds = [];

    // ReSharper disable once ConvertToPrimaryConstructor
    public RecipeBookApiTests(RecipeBookApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetAllRecipes_WhenRecipesExist_ShouldReturnRecipes()
    {
        // Arrange
        using var client = _factory.CreateClient();

        var recipe1 = await Mother.CreateRecipeAsync(client);
        var recipe2 = await Mother.CreateRecipeAsync(client);

        _createdRecipeIds.Add(recipe1.Id);
        _createdRecipeIds.Add(recipe2.Id);

        var expected = new RecipesResponse { Recipes = new[] { recipe1, recipe2 } };

        // Act
        var response = await client.GetAsync(Mother.RecipesApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recipes = await response.Content.ReadFromJsonAsync<RecipesResponse>();
        recipes!.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllRecipes_WhenNoRecipesExist_ShouldReturnNoRecipes()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(Mother.RecipesApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recipes = await response.Content.ReadFromJsonAsync<RecipesResponse>();
        recipes!.Recipes.Should().BeEmpty();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        using var client = _factory.CreateClient();

        foreach (string recipeId in _createdRecipeIds)
        {
            _ = await client.DeleteAsync($"{Mother.RecipesApiBasePath}/{recipeId}");
        }
    }
}