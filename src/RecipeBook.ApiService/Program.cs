using RecipeBook.ApiService;
using RecipeBook.ServiceDefaults;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.OpenTelemetry;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
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
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.AddRecipeBookDefaults();
    }

    var app = builder.Build();
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapDefaultEndpoints();
        app.UseSerilogRequestLogging();
        app.UseGlobalErrorHandling();
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