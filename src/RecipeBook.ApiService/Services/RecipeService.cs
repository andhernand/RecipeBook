using RecipeBook.ApiService.Mapping;
using RecipeBook.ApiService.Repositories;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Services;

public interface IRecipeService
{
    Task<RecipeResponse> CreateAsync(CreateRecipeRequest request, CancellationToken token = default);
    Task<IEnumerable<RecipeResponse>> GetAllAsync(CancellationToken token = default);
    Task<RecipeResponse?> GetByIdAsync(string id, CancellationToken token = default);
    Task<RecipeResponse?> UpdateAsync(string id, UpdateRecipeRequest update, CancellationToken token = default);
    Task<bool> DeleteAsync(string id, CancellationToken token = default);
}

public class RecipeService(IRecipeRepository repository) : IRecipeService
{
    public async Task<RecipeResponse> CreateAsync(CreateRecipeRequest request, CancellationToken token = default)
    {
        var recipe = request.MapToRecipe();
        await repository.CreateAsync(recipe, token);
        return recipe.MapToResponse();
    }

    public async Task<IEnumerable<RecipeResponse>> GetAllAsync(CancellationToken token = default)
    {
        var todos = await repository.GetAllAsync(token);
        return todos.MapToResponse();
    }

    public async Task<RecipeResponse?> GetByIdAsync(string id, CancellationToken token = default)
    {
        var recipe = await repository.GetByIdAsync(id, token);
        return recipe?.MapToResponse();
    }

    public async Task<RecipeResponse?> UpdateAsync(
        string id,
        UpdateRecipeRequest update,
        CancellationToken token = default)
    {
        if (id != update.Id) return null;

        var todo = update.MapToRecipe();
        var updated = await repository.UpdateAsync(id, todo, token);
        return updated?.MapToResponse();
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken token = default)
    {
        var recipe = await repository.DeleteAsync(id, token);
        return recipe is not null;
    }
}