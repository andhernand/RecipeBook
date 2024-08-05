using DotNetEnv;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using RecipeBook.AppHost;

namespace RecipeBook.ApiService.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class RecipeBookApiFactory : WebApplicationFactory<IRecipeBookApiServiceMarker>, IAsyncLifetime
{
    private readonly IHost _app;
    private IResourceBuilder<MongoDBServerResource> Mongo { get; set; }
    private string? _mongoConnectionString;

    public RecipeBookApiFactory()
    {
        Env.TraversePath().Load();

        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(IAppHostMarker).Assembly.FullName, DisableDashboard = true
        };

        var appBuilder = DistributedApplication.CreateBuilder(options);

        Mongo = appBuilder.AddMongoDB("DefaultMongoDb")
            .WithImage("mongodb/mongodb-community-server")
            .WithImageTag("7.0.12-ubuntu2204")
            .WithEnvironment("RECIPE_BOOK_API_USER", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_USER"))
            .WithEnvironment("RECIPE_BOOK_API_PWD", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_PWD"))
            .WithEnvironment("RECIPE_BOOK_API_DATABASE", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_DATABASE"))
            .WithEnvironment("RECIPE_BOOK_API_COLLECTION", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_COLLECTION"))
            .WithBindMount("../../db/mongo-init.sh", "/docker-entrypoint-initdb.d/mongo-init.sh", true);

        appBuilder.AddProject<Projects.RecipeBook_ApiService>("recipe-book-api")
            .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", string.Empty)
            .WithEnvironment("OTEL_EXPORTER_OTLP_HEADERS", "x-otlp-api-key=ede25b98bc12a2d2e879747153b8f9cc")
            .WithEnvironment("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
            .WithReference(Mongo);

        _app = appBuilder.Build();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new KeyValuePair<string, string?>[]
            {
                new($"ConnectionStrings:{Mongo.Resource.Name}", _mongoConnectionString),
                new("OTEL_EXPORTER_OTLP_ENDPOINT", string.Empty),
                new("OTEL_EXPORTER_OTLP_HEADERS", "x-otlp-api-key=ede25b98bc12a2d2e879747153b8f9cc"),
                new("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc")
            });
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _app.StartAsync();
        _mongoConnectionString = await Mongo.Resource.ConnectionStringExpression.GetValueAsync(new CancellationToken());
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _app.StopAsync();
        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _app.Dispose();
        }
    }

    private static string GetEnvironmentVariableOrThrow(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName) ??
               throw new InvalidOperationException($"Environment variable not set: {variableName}");
    }
}