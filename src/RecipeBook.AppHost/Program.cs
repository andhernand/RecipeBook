using DotNetEnv;

using static RecipeBook.AppHost.Utilities;

Env.TraversePath().Load();

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("DefaultMongoDb")
    .WithImage("mongodb/mongodb-community-server", "7.0.12-ubuntu2204")
    .WithEnvironment("RECIPE_BOOK_API_USER", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_USER"))
    .WithEnvironment("RECIPE_BOOK_API_PWD", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_PWD"))
    .WithEnvironment("RECIPE_BOOK_API_DATABASE", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_DATABASE"))
    .WithEnvironment("RECIPE_BOOK_API_COLLECTION", GetEnvironmentVariableOrThrow("RECIPE_BOOK_API_COLLECTION"))
    .WithBindMount("../../db/mongo-init.sh", "/docker-entrypoint-initdb.d/mongo-init.sh", true)
    .WithMongoExpress(x => x.WithImage("mongo-express", "1.0.2-20-alpine3.19"));

var cache = builder.AddRedis("cache").WithImage("redis", "7.4.0");

var apiService = builder.AddProject<Projects.RecipeBook_ApiService>("recipe-book-api")
    .WithReference(db);

builder.AddProject<Projects.RecipeBook_Web>("recipe-book-web")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();