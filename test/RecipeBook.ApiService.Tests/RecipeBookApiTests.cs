using System.Net.Http.Json;

using FluentAssertions;

using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Tests;

public class RecipeBookApiTests : IClassFixture<RecipeBookApiFactory>
{
    private readonly RecipeBookApiFactory _factory;

    // ReSharper disable once ConvertToPrimaryConstructor
    public RecipeBookApiTests(RecipeBookApiFactory factory)
    {
        _factory = factory;
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
}