﻿using System.Net.Http.Json;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Tests;

public class RecipeBookApiTests(RecipeBookApiFactory factory) : IClassFixture<RecipeBookApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly List<string> _createdRecipeIds = [];

    [Fact]
    public async Task CreateRecipe_WhenRecipeIsValid_ShouldCreateRecipe()
    {
        // Arrange
        var request = Mother.GenerateCreateRecipeRequest();

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var actual = await response.Content.ReadFromJsonAsync<RecipeResponse>();
        _createdRecipeIds.Add(actual!.Id);
        response.Headers.Location.Should().Be($"http://localhost{Mother.RecipesApiBasePath}/{actual.Id}");

        actual.Title.Should().Be(request.Title);
        actual.Description.Should().Be(request.Description);
        actual.Ingredients.Should().BeEquivalentTo(request.Ingredients);
        actual.Directions.Should().BeEquivalentTo(request.Directions);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeTitleIsInValid_ShouldReturnBadRequest()
    {
        // Arrange
        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Title", ["'Title' must not be empty."] }
        });
        var request = Mother.GenerateCreateRecipeRequest(title: "");

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeDescriptionIsInValid_ShouldReturnBadRequest()
    {
        // Arrange
        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Description", ["'Description' must not be empty."] }
        });
        var request = Mother.GenerateCreateRecipeRequest(description: "");

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeIngredientsAreEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Ingredients", ["'Ingredients' must not be empty."] }
        });

        var request = Mother.GenerateCreateRecipeRequest(ingredients: []);

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeIngredientsContainAnEmptyString_ShouldReturnBadRequest()
    {
        // Arrange
        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Ingredients[2]", ["Ingredient at index 2 must not be empty."] }
        });

        var request = Mother.GenerateCreateRecipeRequest(ingredients: new[] { "oregano", "garlic", "" });

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeDirectionsAreEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Directions", ["'Directions' must not be empty."] }
        });

        var request = Mother.GenerateCreateRecipeRequest(directions: []);

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task CreateRecipe_WhenRecipeDirectionsContainAnEmptyString_ShouldReturnBadRequest()
    {
        // Arrange
        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Directions[1]", ["Direction at index 1 must not be empty."] }
        });

        var request = Mother.GenerateCreateRecipeRequest(directions: new[] { "mix things", "", "cook things" });

        // Act
        var response = await _client.PostAsJsonAsync($"{Mother.RecipesApiBasePath}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllRecipes_WhenRecipesExist_ShouldReturnRecipes()
    {
        // Arrange
        var recipe1 = await Mother.CreateRecipeAsync(_client);
        var recipe2 = await Mother.CreateRecipeAsync(_client);

        _createdRecipeIds.Add(recipe1.Id);
        _createdRecipeIds.Add(recipe2.Id);

        var expected = new[] { recipe1, recipe2 };

        // Act
        var response = await _client.GetAsync(Mother.RecipesApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recipes = await response.Content.ReadFromJsonAsync<IEnumerable<RecipeResponse>>();
        recipes!.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllRecipes_WhenNoRecipesExist_ShouldReturnNoRecipes()
    {
        // Act
        var response = await _client.GetAsync(Mother.RecipesApiBasePath);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var recipes = await response.Content.ReadFromJsonAsync<IEnumerable<RecipeResponse>>();
        recipes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeIsFound_ShouldReturnRecipeResponse()
    {
        // Arrange
        var expected = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(expected.Id);

        // Act
        var response = await _client.GetAsync($"{Mother.RecipesApiBasePath}/{expected.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<RecipeResponse>();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeDoesNotExists_ShouldReturns404NotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();

        // Act
        var response = await _client.GetAsync($"{Mother.RecipesApiBasePath}/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRecipeById_WhenRecipeIdIsInvalid_ShouldReturnsBadRequest()
    {
        // Act
        var response = await _client.GetAsync($"{Mother.RecipesApiBasePath}/yoda");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeIsValid_ShouldUpdateRecipe()
    {
        // Arrange
        var inserted = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(inserted.Id);

        var request = new UpdateRecipeRequest
        {
            Id = inserted.Id,
            Title = inserted.Title,
            Description = "I changed this description",
            Ingredients = inserted.Ingredients,
            Directions = inserted.Directions
        };

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var actual = await response.Content.ReadFromJsonAsync<RecipeResponse>();
        actual!.Id.Should().Be(inserted.Id);
        actual.Title.Should().Be(inserted.Title);
        actual.Description.Should().Be(request.Description);
        actual.Ingredients.Should().BeEquivalentTo(inserted.Ingredients);
        actual.Directions.Should().BeEquivalentTo(inserted.Directions);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeTitleIsInValid_ShouldBadRequest()
    {
        // Arrange
        var inserted = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(inserted.Id);

        var request = new UpdateRecipeRequest
        {
            Id = inserted.Id,
            Title = "",
            Description = inserted.Description,
            Ingredients = inserted.Ingredients,
            Directions = inserted.Directions
        };

        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Title", ["'Title' must not be empty."] }
        });

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeDescriptionIsInValid_ShouldReturnBadRequest()
    {
        // Arrange
        var inserted = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(inserted.Id);

        var request = new UpdateRecipeRequest
        {
            Id = inserted.Id,
            Title = inserted.Title,
            Description = "",
            Ingredients = inserted.Ingredients,
            Directions = inserted.Directions
        };

        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Description", ["'Description' must not be empty."] }
        });

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeIdIsInValid_ShouldReturnBadRequest()
    {
        // Arrange
        var request = Mother.GenerateUpdateRecipeRequest();

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeIdIsNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var request = Mother.GenerateUpdateRecipeRequest(id: id.ToString());

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeIngredientsAreEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(created.Id);

        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Ingredients", ["'Ingredients' must not be empty."] }
        });

        var request = Mother.GenerateUpdateRecipeRequest(id: created.Id, ingredients: []);

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeIngredientsContainAnEmptyString_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(created.Id);

        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Ingredients[2]", ["Ingredient at index 2 must not be empty."] }
        });

        var request = Mother.GenerateUpdateRecipeRequest(id: created.Id, ingredients: ["oregano", "garlic", ""]);

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeDirectionsAreEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(created.Id);

        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Directions", ["'Directions' must not be empty."] }
        });

        var request = Mother.GenerateUpdateRecipeRequest(id: created.Id, directions: []);

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeDirectionsContainAnEmptyString_ShouldReturnBadRequest()
    {
        // Arrange
        var created = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(created.Id);

        var expected = Mother.GenerateValidationProblemDetails(new Dictionary<string, string[]>
        {
            { "Directions[1]", ["Direction at index 1 must not be empty."] }
        });

        var request = Mother.GenerateUpdateRecipeRequest(id: created.Id, directions: ["mix things", "", "cook things"]);

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var errors = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        errors.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRepipeIdDoesNotMatch_ShouldReturnNotFound()
    {
        // Arrange
        var created = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(created.Id);
        var differentId = ObjectId.GenerateNewId();

        var request = Mother.GenerateUpdateRecipeRequest(
            differentId.ToString(), created.Title, created.Description, ["clean", "eat"], ["wash", "cook"]);

        // Act
        var response = await _client.PutAsJsonAsync(Mother.RecipesApiBasePath, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRecipe_WhenRecipeExists_ShouldReturnNoContent()
    {
        // Arrange
        var recipe = await Mother.CreateRecipeAsync(_client);
        _createdRecipeIds.Add(recipe.Id);

        // Act
        var response = await _client.DeleteAsync($"{Mother.RecipesApiBasePath}/{recipe.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteRecipe_WhenRecipeDoesNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var id = ObjectId.GenerateNewId();

        // Act
        var response = await _client.DeleteAsync($"{Mother.RecipesApiBasePath}/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRecipe_WhenRecipeIdIsInvalid_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.DeleteAsync($"{Mother.RecipesApiBasePath}/yoda");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (string recipeId in _createdRecipeIds)
        {
            _ = await _client.DeleteAsync($"{Mother.RecipesApiBasePath}/{recipeId}");
        }
    }
}