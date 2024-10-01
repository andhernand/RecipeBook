using Microsoft.Extensions.Options;

using MongoDB.Driver;

using RecipeBook.ApiService.Models;
using RecipeBook.ApiService.Options;

namespace RecipeBook.ApiService.Repositories;

public interface IRecipeRepository
{
    Task CreateAsync(Recipe recipe, CancellationToken token = default);
    Task<IEnumerable<Recipe>> GetAllAsync(CancellationToken token = default);
    Task<Recipe?> GetByIdAsync(string id, CancellationToken token = default);
    Task<Recipe?> UpdateAsync(Recipe update, CancellationToken token = default);
    Task<Recipe?> DeleteAsync(string id, CancellationToken token = default);
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

    public async Task CreateAsync(Recipe recipe, CancellationToken token = default)
    {
        _logger.LogInformation("Creating {RecipeTitle} Recipe", recipe.Title);

        await _recipeCollection.InsertOneAsync(recipe, cancellationToken: token);
    }

    public async Task<IEnumerable<Recipe>> GetAllAsync(CancellationToken token = default)
    {
        _logger.LogInformation("Getting all Recipes");

        using var cursor = await _recipeCollection.FindAsync(
            Builders<Recipe>.Filter.Empty,
            cancellationToken: token);

        return await cursor.ToListAsync(token);
    }

    public async Task<Recipe?> GetByIdAsync(string id, CancellationToken token = default)
    {
        _logger.LogInformation("Getting Recipe {Id}", id);

        using var cursor = await _recipeCollection.FindAsync(
            Builders<Recipe>.Filter.Eq(r => r.Id, id),
            cancellationToken: token);

        return await cursor.FirstOrDefaultAsync(token);
    }

    public async Task<Recipe?> UpdateAsync(Recipe update, CancellationToken token = default)
    {
        _logger.LogInformation("Updating Recipe {Id}", update.Id);

        return await _recipeCollection.FindOneAndReplaceAsync(
            Builders<Recipe>.Filter.Eq(r => r.Id, update.Id),
            update,
            new FindOneAndReplaceOptions<Recipe> { ReturnDocument = ReturnDocument.After },
            cancellationToken: token);
    }

    public async Task<Recipe?> DeleteAsync(string id, CancellationToken token = default)
    {
        _logger.LogInformation("Deleting Recipe {Id}", id);

        return await _recipeCollection.FindOneAndDeleteAsync(
            Builders<Recipe>.Filter.Eq(r => r.Id, id),
            cancellationToken: token);
    }
}