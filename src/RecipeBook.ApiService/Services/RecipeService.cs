using RecipeBook.ApiService.Models;
using RecipeBook.ApiService.Repositories;

namespace RecipeBook.ApiService.Services;

public interface IRecipeService
{
    Task<Recipe> CreateRecipeAsync(Recipe recipe, CancellationToken token);
    Task<IEnumerable<Recipe>> GetAllRecipesAsync(CancellationToken token);
    Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken token);
    Task<Recipe?> UpdateRecipeAsync(string id, Recipe update, CancellationToken token);
    Task<Recipe?> DeleteRecipeAsync(string id, CancellationToken token);
    Task<bool> ExistsById(string id, CancellationToken token);
}

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public Task<Recipe> CreateRecipeAsync(Recipe recipe, CancellationToken token)
    {
        return _repository.CreateRecipeAsync(recipe, token);
    }

    public Task<IEnumerable<Recipe>> GetAllRecipesAsync(CancellationToken token)
    {
        return _repository.GetAllRecipesAsync(token);
    }

    public Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken token)
    {
        return _repository.GetRecipeByIdAsync(id, token);
    }

    public Task<Recipe?> UpdateRecipeAsync(string id, Recipe update, CancellationToken token)
    {
        return _repository.UpdateRecipeAsync(id, update, token);
    }

    public Task<Recipe?> DeleteRecipeAsync(string id, CancellationToken token)
    {
        return _repository.DeleteRecipeAsync(id, token);
    }

    public Task<bool> ExistsById(string id, CancellationToken token)
    {
        return _repository.ExistsById(id, token);
    }
}