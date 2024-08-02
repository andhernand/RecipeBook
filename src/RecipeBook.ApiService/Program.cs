using MongoDB.Driver;

using RecipeBook.ApiService.Options;
using RecipeBook.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();

builder.Services.AddOptions<RecipeBookDatabaseOptions>()
    .BindConfiguration(RecipeBookDatabaseOptions.Key)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.AddMongoDBClient("DefaultMongoDb",
    configureClientSettings: x =>
    {
        var dbOptions = new RecipeBookDatabaseOptions();
        builder.Configuration.GetSection(RecipeBookDatabaseOptions.Key).Bind(dbOptions);

        x.ApplicationName = "recipe-book-api";
        // TODO: think about grabbing credential values from Environment Variables.
        x.Credential = MongoCredential.CreateCredential(
            dbOptions.DatabaseName,
            dbOptions.Username,
            dbOptions.Password);
    });

var app = builder.Build();

app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
