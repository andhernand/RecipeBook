using FluentValidation;

using MongoDB.Driver;

using RecipeBook.ApiService.Endpoints.Recipes;
using RecipeBook.ApiService.Options;
using RecipeBook.ApiService.Repositories;
using RecipeBook.ApiService.Services;

namespace RecipeBook.ApiService;

public static class Extensions
{
    public static void AddRecipeBookDefaults(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("DefaultMongoDb", configureClientSettings: x =>
        {
            var dbOptions = new DatabaseOptions();
            builder.Configuration.GetSection(DatabaseOptions.Key).Bind(dbOptions);

            x.ApplicationName = "recipe-book-api";
            x.Credential = MongoCredential.CreateCredential(
                dbOptions.DatabaseName,
                dbOptions.Username,
                dbOptions.Password);
        });

        builder.Services.AddOptions<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddProblemDetails();
        builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();
        builder.Services.AddTransient<IRecipeService, RecipeService>();
        builder.Services.AddValidatorsFromAssemblyContaining<IRecipeBookApiServiceMarker>(ServiceLifetime.Singleton);
    }

    public static void UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();
    }

    public static void MapApiEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapCreateRecipe();
        builder.MapGetAllRecipes();
        builder.MapGetRecipeById();
        builder.MapUpdateRecipe();
        builder.MapDeleteRecipe();
    }
}