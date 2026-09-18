using GameOfLife.Domain.Observability;
using GameOfLife.Infrastructure;
using GameOfLife.Application;
using GameOfLife.Application.CreateUniverse;
using GameOfLife.Application.GetFinalState;
using GameOfLife.Application.GetGeneration;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Api.Controllers;
using GameOfLife.Api.ErrorHandling;
using GameOfLife.Api.Options;
using GameOfLife.Api.Swagger;
using GameOfLife.Api.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

// Model binding failures and FluentValidation failures both become this one RFC 7807 body. The
// factory also strips the binder's default text, which names CLR types and JSON byte offsets.
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(ValidationProblems.Create(context.ModelState))
        {
            ContentTypes = { "application/problem+json" },
        });

// Validators are invoked explicitly by the controller. FluentValidation deprecated its MVC
// auto-validation pipeline and ships no filter replacement, so registration is all that is wired here.
builder.Services.AddValidatorsFromAssemblyContaining<UploadBoardRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.OperationFilter<GliderExampleOperationFilter>());

builder.Services.AddSingleton(TimeProvider.System);

var gameOfLifeSection = builder.Configuration.GetSection(GameOfLifeOptions.SectionName);
builder.Services
    .AddOptions<GameOfLifeOptions>()
    .Bind(gameOfLifeSection)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddScoped<ICommandHandler<CreateUniverseCommand>, CreateUniverseCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetUniverseQuery, UniverseView>, GetUniverseQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetGenerationQuery, PatternView>, GetGenerationQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetFinalStateQuery, FinalStateView>, GetFinalStateQueryHandler>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"]);

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
        .AddMeter("Microsoft.AspNetCore.RateLimiting")
        .AddConsoleExporter());

// The one admission control in the process: it bounds how many CPU-bound evaluations run at once,
// which the input caps in GameOfLifeOptions cannot do (those bound the cost of a single request).
// QueueLimit 0 rejects immediately rather than queueing, because a request answered after the client
// has given up has cost a core for nothing. See docs/design.md §8.4.
builder.Services.AddRateLimiter(options =>
{
    var permits = gameOfLifeSection.Get<GameOfLifeOptions>()?.ResolvedMaxConcurrentEvaluations
        ?? Environment.ProcessorCount;

    options.AddConcurrencyLimiter(BoardsController.EvaluationPolicy, limiter =>
    {
        limiter.PermitLimit = permits;
        limiter.QueueLimit = 0;
    });

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
                Detail = "Too many evaluations are in progress. Retry shortly.",
            },
            options: null,
            contentType: "application/problem+json",
            ct);
    };
});

var app = builder.Build();

app.Services.EnsureDatabaseCreated();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRateLimiter();

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
