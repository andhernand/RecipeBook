using Microsoft.Extensions.Options;

using MongoDB.Driver;

using RecipeBook.ApiService.Models;
using RecipeBook.ApiService.Options;

namespace RecipeBook.ApiService.Repositories;

public interface IRecipeRepository
{
    Task CreateRecipeAsync(Recipe recipe, CancellationToken token = default);
    Task<IEnumerable<Recipe>> GetAllRecipesAsync(CancellationToken token);
    Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken token);
    Task<Recipe?> UpdateRecipeAsync(string id, Recipe update, CancellationToken token);
    Task<Recipe?> DeleteRecipeAsync(string id, CancellationToken token = default);
    Task<bool> ExistsById(string id, CancellationToken token);
}

public class RecipeRepository : IRecipeRepository
{
    private readonly IMongoCollection<Recipe> _recipeCollection;
    private readonly ILogger<RecipeRepository> _logger;

    public RecipeRepository(
        IMongoClient mongoClient,
        IOptions<DatabaseOptions> dbSettings,
        ILogger<RecipeRepository> logger)
    {
        var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DatabaseName);
        _recipeCollection = mongoDatabase.GetCollection<Recipe>(dbSettings.Value.CollectionName);
        _logger = logger;
    }

    public async Task CreateRecipeAsync(Recipe recipe, CancellationToken token = default)
    {
        _logger.LogInformation("Creating {RecipeTitle} Recipe", recipe.Title);

        try
        {
            token.ThrowIfCancellationRequested();

            await _recipeCollection.InsertOneAsync(recipe, cancellationToken: token);
            _logger.LogInformation("Created Recipe {Id}", recipe.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating a recipe for {RecipeTitle}", recipe.Title);
            throw;
        }
    }

    public async Task<IEnumerable<Recipe>> GetAllRecipesAsync(CancellationToken token)
    {
        _logger.LogInformation("Getting all Recipes");

        try
        {
            token.ThrowIfCancellationRequested();

            using var recipes = await _recipeCollection.FindAsync(
                Builders<Recipe>.Filter.Empty,
                cancellationToken: token);

            var result = await recipes.ToListAsync(token);
            _logger.LogInformation("Fetched {Count} recipes", result.Count);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting all recipes");
            throw;
        }
    }

    public async Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken token)
    {
        _logger.LogInformation("Getting Recipe {Id}", id);

        try
        {
            token.ThrowIfCancellationRequested();

            using var recipe = await _recipeCollection.FindAsync(
                Builders<Recipe>.Filter.Eq(r => r.Id, id),
                cancellationToken: token);

            var result = await recipe.FirstOrDefaultAsync(token);
            _logger.LogInformation("Recipe {Id} was found: {IsFound}", id, result is not null);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting recipe {Id}", id);
            throw;
        }
    }

    public async Task<Recipe?> UpdateRecipeAsync(string id, Recipe update, CancellationToken token)
    {
        _logger.LogInformation("Updating Recipe {Id}", id);

        try
        {
            var result = await _recipeCollection.FindOneAndReplaceAsync(
                Builders<Recipe>.Filter.Eq(r => r.Id, id),
                update,
                new FindOneAndReplaceOptions<Recipe> { ReturnDocument = ReturnDocument.After },
                cancellationToken: token);

            _logger.LogInformation("Updated Recipe {Id}: {WasUpdated}", id, result is not null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating recipe {Id}", id);
            throw;
        }
    }

    public async Task<Recipe?> DeleteRecipeAsync(string id, CancellationToken token = default)
    {
        _logger.LogInformation("Deleting Recipe {Id}", id);

        try
        {
            token.ThrowIfCancellationRequested();

            var result = await _recipeCollection.FindOneAndDeleteAsync(
                Builders<Recipe>.Filter.Eq(r => r.Id, id),
                cancellationToken: token);

            _logger.LogInformation("Deleted Recipe {Id}: {IsDeleted}", id, result is not null);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting recipe {Id}", id);
            throw;
        }
    }

    public Task<bool> ExistsById(string id, CancellationToken token)
    {
        _logger.LogInformation("Checking if Recipe {Id} exists", id);

        try
        {
            var count = _recipeCollection
                .Find(Builders<Recipe>.Filter.Eq(r => r.Id, id))
                .CountDocuments(token);
            return Task.FromResult(count > 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while checking for the existence of recipe {Id}", id);
            throw;
        }
    }
}