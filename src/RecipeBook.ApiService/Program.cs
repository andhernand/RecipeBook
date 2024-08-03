using RecipeBook.ApiService;
using RecipeBook.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);
{
    builder.AddServiceDefaults();
    builder.AddRecipeBookDefaults();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseGlobalErrorHandling();
    app.MapDefaultEndpoints();
    app.MapApiEndpoints();
}

app.Run();