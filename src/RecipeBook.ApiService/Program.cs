using System.Diagnostics;

using MongoDB.Driver;

using RecipeBook.ApiService.Options;
using RecipeBook.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

builder.Services.AddOptions<RecipeBookDatabaseOptions>()
    .BindConfiguration(RecipeBookDatabaseOptions.Key)
    .ValidateDataAnnotations()
    .ValidateOnStart();

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

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();

app.Run();