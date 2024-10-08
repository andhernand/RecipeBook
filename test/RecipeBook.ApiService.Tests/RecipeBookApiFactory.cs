﻿using DotNetEnv;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using RecipeBook.AppHost;

namespace RecipeBook.ApiService.Tests;

public class RecipeBookApiFactory : WebApplicationFactory<IRecipeBookApiServiceMarker>, IAsyncLifetime
{
    private static readonly string ApiUsername;
    private static readonly string ApiPassword;
    private static readonly string ApiDatabase;
    private static readonly string ApiCollection;
    private static readonly string MongoInitScriptPath;

    private readonly IHost _app;
    private readonly IResourceBuilder<MongoDBServerResource> _mongo;
    private readonly ResourceNotificationService _resourceNotificationService;

    private string? _mongoConnectionString;

    public RecipeBookApiFactory()
    {
        var options = new DistributedApplicationOptions
        {
            AssemblyName = typeof(IAppHostMarker).Assembly.FullName, DisableDashboard = true
        };

        var appBuilder = DistributedApplication.CreateBuilder(options);

        appBuilder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _mongo = appBuilder.AddMongoDB("DefaultMongoDb")
            .WithImage("mongodb/mongodb-community-server")
            .WithImageTag("7.0.12-ubuntu2204")
            .WithEnvironment("RECIPE_BOOK_API_USER", ApiUsername)
            .WithEnvironment("RECIPE_BOOK_API_PWD", ApiPassword)
            .WithEnvironment("RECIPE_BOOK_API_DATABASE", ApiDatabase)
            .WithEnvironment("RECIPE_BOOK_API_COLLECTION", ApiCollection)
            .WithBindMount(MongoInitScriptPath, "/docker-entrypoint-initdb.d/mongo-init.sh", true);

        _app = appBuilder.Build();

        _resourceNotificationService = _app.Services.GetRequiredService<ResourceNotificationService>();
    }

    static RecipeBookApiFactory()
    {
        Env.TraversePath().Load();
        ApiUsername = GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_USER");
        ApiPassword = GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_PWD");
        ApiDatabase = GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_DATABASE");
        ApiCollection = GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_COLLECTION");

        MongoInitScriptPath = Path.Combine(
            DirectoryFinder.GetDirectoryContaining("RecipeBook.sln"),
            "db",
            "mongo-init.sh");

        if (!File.Exists(MongoInitScriptPath))
        {
            throw new FileNotFoundException($"{MongoInitScriptPath} file not found");
        }
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection([
                new($"ConnectionStrings:{_mongo.Resource.Name}", _mongoConnectionString),
                new("DatabaseOptions:DatabaseName", ApiDatabase),
                new("DatabaseOptions:CollectionName", ApiCollection),
                new("DatabaseOptions:Username", ApiUsername),
                new("DatabaseOptions:Password", ApiPassword)
            ]);
        });

        return base.CreateHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _app.StartAsync();
        _mongoConnectionString = await _mongo.Resource
            .ConnectionStringExpression.GetValueAsync(new CancellationToken());
        await _resourceNotificationService.WaitForResourceAsync(_mongo.Resource.Name, KnownResourceStates.Running);
    }

    public new async Task DisposeAsync()
    {
        await _app.StopAsync();

        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _app.Dispose();
        }

        await base.DisposeAsync();
    }

    private static string GetEnvironmentVariableOrThrow(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName) ??
               throw new InvalidOperationException($"Environment variable not set: {variableName}");
    }
}