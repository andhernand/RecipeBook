using RecipeBook.ApiService;
using RecipeBook.ServiceDefaults;

using Serilog;
using Serilog.Sinks.OpenTelemetry;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Properties} {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProcessId()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .Enrich.WithProperty("Application", "Recipe Book API")
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Recipe Book API");

    var builder = WebApplication.CreateBuilder(args);
    {
        builder.Host.UseSerilog((context, configuration) => configuration
            .WriteTo.Console(outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Properties} {Message:lj}{NewLine}{Exception}")
            .WriteTo.OpenTelemetry(x =>
            {
                var otlpEndpoint = context.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                {
                    x.Endpoint = otlpEndpoint;

                    var otelHeader = context.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]!.Split('=');
                    x.Headers = new Dictionary<string, string> { { otelHeader[0], otelHeader[1] } };

                    x.Protocol = context.Configuration["OTEL_EXPORTER_OTLP_PROTOCOL"] == "grpc"
                        ? OtlpProtocol.Grpc
                        : OtlpProtocol.HttpProtobuf;
                }
            })
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("Application", "Recipe Book API")
        );

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

        app.UseSerilogRequestLogging();
        app.UseGlobalErrorHandling();
        app.MapDefaultEndpoints();
        app.MapApiEndpoints();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}