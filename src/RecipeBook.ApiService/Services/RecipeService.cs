using RecipeBook.ApiService.Mapping;
using RecipeBook.ApiService.Models;
using RecipeBook.ApiService.Repositories;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Services;

public interface IRecipeService
{
    Task<RecipeResponse> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken token = default);
    Task<IEnumerable<Recipe>> GetAllRecipesAsync(CancellationToken token);
    Task<Recipe?> GetRecipeByIdAsync(string id, CancellationToken token);
    Task<Recipe?> UpdateRecipeAsync(string id, Recipe update, CancellationToken token);
    Task<Recipe?> DeleteRecipeAsync(string id, CancellationToken token);
}

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;

    public RecipeService(IRecipeRepository repository)
    {
        _repository = repository;
    }

    public async Task<RecipeResponse> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken token = default)
    {
        var recipe = request.MapToRecipe();
        await _repository.CreateRecipeAsync(recipe, token);
        return recipe.MapToResponse();
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
}