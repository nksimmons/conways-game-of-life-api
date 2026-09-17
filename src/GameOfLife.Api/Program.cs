using System.Threading.RateLimiting;
using GameOfLife.Domain.Observability;
using GameOfLife.Infrastructure;
using GameOfLife.Application;
using GameOfLife.Application.CreateUniverse;
using GameOfLife.Application.GetFinalState;
using GameOfLife.Application.GetGeneration;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Api.Concurrency;
using GameOfLife.Api.ErrorHandling;
using GameOfLife.Api.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

// Controllers are resolved through DI (AddControllersAsServices) so that ValidateOnBuild sees their
// constructor dependencies: a handler registration missing from this file fails the process at boot,
// not on the first request that needs it. See AGENTS.md §1.3.
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateOnBuild = true;
    options.ValidateScopes = true;
});

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter()));

builder.Services.AddControllers().AddControllersAsServices();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(TimeProvider.System);

builder.Services
    .AddOptions<GameOfLifeOptions>()
    .Bind(builder.Configuration.GetSection(GameOfLifeOptions.SectionName))
    .ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<GameOfLifeOptions>, GameOfLifeOptionsValidator>();

builder.Services.AddSingleton<IEvaluationAdmissionGate, EvaluationAdmissionGate>();

builder.Services.AddScoped<ICommandHandler<CreateUniverseCommand>, CreateUniverseCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetUniverseQuery, UniverseView>, GetUniverseQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetGenerationQuery, PatternView>, GetGenerationQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetFinalStateQuery, FinalStateView>, GetFinalStateQueryHandler>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(GameOfLifeDiagnostics.Name))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(GameOfLifeDiagnostics.Name)
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter(GameOfLifeDiagnostics.Name)
        .AddConsoleExporter());

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, ct) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "1";
        context.HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Type = "https://gameoflife.example/problems/admission-rejected",
                Title = "The server is at capacity.",
                Status = StatusCodes.Status503ServiceUnavailable,
            },
            options: null,
            contentType: "application/problem+json",
            ct).ConfigureAwait(false);
    };

    // An outer bound on total in-flight requests, distinct from the per-evaluation admission gate
    // (Web/Concurrency/EvaluationAdmissionGate) which bounds only the CPU-bound endpoints.
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
        RateLimitPartition.GetConcurrencyLimiter(
            partitionKey: "global",
            factory: _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = Environment.ProcessorCount * 8,
                QueueLimit = 0,
            }));
});

var app = builder.Build();

app.Services.EnsureDatabaseCreated();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
});

app.Run();

/// <summary>Entry point partial class, exposed so <c>WebApplicationFactory&lt;Program&gt;</c> can bootstrap it in functional tests.</summary>
public partial class Program
{
}

