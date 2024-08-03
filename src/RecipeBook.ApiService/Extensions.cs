using System.Diagnostics;

using FluentValidation;

using MongoDB.Driver;

using RecipeBook.ApiService.Options;
using RecipeBook.ApiService.Repositories;
using RecipeBook.ApiService.Services;

namespace RecipeBook.ApiService;

public static class Extensions
{
    public static IHostApplicationBuilder AddRecipeBookDefaults(this IHostApplicationBuilder builder)
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

        builder.Services.AddProblemDetails(x =>
        {
            x.CustomizeProblemDetails = context =>
            {
                if (!context.ProblemDetails.Extensions.ContainsKey("traceId"))
                {
                    context.ProblemDetails.Extensions["traceId"] =
                        Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                }
            };
        });

        builder.Services.AddSingleton<IRecipeService, RecipeService>();
        builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();
        builder.Services.AddValidatorsFromAssemblyContaining<IRecipeBookApiServiceMarker>(ServiceLifetime.Singleton);

        return builder;
    }

    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }
}