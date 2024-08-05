using RecipeBook.ApiService;
using RecipeBook.ServiceDefaults;

using Serilog;
using Serilog.Sinks.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console(
            outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Properties} {Message:lj}{NewLine}{Exception}")
        .WriteTo.OpenTelemetry(x =>
        {
            var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

            if (!string.IsNullOrWhiteSpace(otlpEndpoint))
            {
                x.Endpoint = otlpEndpoint;

                var otelHeader = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]!.Split('=');
                x.Headers = new Dictionary<string, string> { { otelHeader[0], otelHeader[1] } };

                x.Protocol = builder.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"] == "grpc"
                    ? OtlpProtocol.Grpc
                    : OtlpProtocol.HttpProtobuf;
            }
        })
        .Enrich.FromLogContext()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProcessId()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "Recipe Book API")
        .CreateLogger();

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