using RecipeBook.ApiService.Mapping;
using RecipeBook.ApiService.Repositories;
using RecipeBook.Contracts.Requests;
using RecipeBook.Contracts.Responses;

namespace RecipeBook.ApiService.Services;

public interface IRecipeService
{
    Task<RecipeResponse> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken token = default);
    Task<IEnumerable<RecipeResponse>> GetAllRecipesAsync(CancellationToken token = default);
    Task<RecipeResponse?> GetRecipeByIdAsync(string id, CancellationToken token = default);
    Task<RecipeResponse?> UpdateRecipeAsync(string id, UpdateRecipeRequest update, CancellationToken token = default);
    Task<bool> DeleteRecipeAsync(string id, CancellationToken token = default);
}

public class RecipeService(IRecipeRepository repository) : IRecipeService
{
    public async Task<RecipeResponse> CreateRecipeAsync(CreateRecipeRequest request, CancellationToken token = default)
    {
        var recipe = request.MapToRecipe();
        await repository.CreateRecipeAsync(recipe, token);
        return recipe.MapToResponse();
    }

    public async Task<IEnumerable<RecipeResponse>> GetAllRecipesAsync(CancellationToken token = default)
    {
        var todos = await repository.GetAllRecipesAsync(token);
        return todos.MapToResponse();
    }

    public async Task<RecipeResponse?> GetRecipeByIdAsync(string id, CancellationToken token = default)
    {
        var recipe = await repository.GetRecipeByIdAsync(id, token);
        return recipe?.MapToResponse();
    }

    public async Task<RecipeResponse?> UpdateRecipeAsync(
        string id,
        UpdateRecipeRequest update,
        CancellationToken token = default)
    {
        var todo = update.MapToRecipe();
        var updated = await repository.UpdateRecipeAsync(id, todo, token);
        return updated?.MapToResponse();
    }

    public async Task<bool> DeleteRecipeAsync(string id, CancellationToken token = default)
    {
        var recipe = await repository.DeleteRecipeAsync(id, token);
        return recipe is not null;
    }
}