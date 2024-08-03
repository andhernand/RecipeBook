using System.Diagnostics;

using MongoDB.Driver;

using RecipeBook.ApiService.Options;

namespace RecipeBook.ApiService;

public static class Extensions
{
    public static IHostApplicationBuilder AddRecipeBookDefaults(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDBClient("DefaultMongoDb", configureClientSettings: x =>
        {
            var dbOptions = new RecipeBookDatabaseOptions();
            builder.Configuration.GetSection(RecipeBookDatabaseOptions.Key).Bind(dbOptions);

            x.ApplicationName = "recipe-book-api";
            x.Credential = MongoCredential.CreateCredential(
                dbOptions.DatabaseName,
                dbOptions.Username,
                dbOptions.Password);
        });

        builder.Services.AddOptions<RecipeBookDatabaseOptions>()
            .BindConfiguration(RecipeBookDatabaseOptions.Key)
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

        return builder;
    }

    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler();

        return app;
    }
}