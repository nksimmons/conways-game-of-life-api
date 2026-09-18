using FluentValidation;
using GameOfLife.Api.Controllers;
using GameOfLife.Api.ErrorHandling;
using GameOfLife.Api.Options;
using GameOfLife.Api.Swagger;
using GameOfLife.Api.Validation;
using GameOfLife.Application;
using GameOfLife.Application.CreateUniverse;
using GameOfLife.Application.GetFinalState;
using GameOfLife.Application.GetGeneration;
using GameOfLife.Application.GetUniverse;
using GameOfLife.Domain.Observability;
using GameOfLife.Domain.Rules;
using GameOfLife.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
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

// Levels come from the Serilog section of appsettings, which differs per environment: Debug for our
// own code in Development and Staging, Information in Production, with the framework held at Warning
// everywhere except lifetime messages. The rendered console is for a human reading a terminal;
// anything else is a log aggregator that has to parse it, so it gets JSON.
builder.Host.UseSerilog((context, _, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

    if (context.HostingEnvironment.IsDevelopment())
    {
        configuration.WriteTo.Console();
    }
    else
    {
        configuration.WriteTo.Console(new JsonFormatter());
    }
});

builder.Services.AddControllers().AddControllersAsServices();

// Model binding failures and FluentValidation failures both become this one RFC 7807 body. The
// factory also strips the binder's default text, which names CLR types and JSON byte offsets.
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.InvalidModelStateResponseFactory = context =>
        new BadRequestObjectResult(context.ModelState.ToProblemDetails())
        {
            ContentTypes = { "application/problem+json" }
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
    .Validate(options => new RuleId(options.DefaultRule).IsSupported(),
        "GameOfLife:DefaultRule must identify a supported Life rule.")
    .ValidateOnStart();

builder.Services.AddScoped<ICommandHandler<CreateUniverseCommand>, CreateUniverseCommandHandler>();
builder.Services.AddScoped<IQueryHandler<GetUniverseQuery, UniverseView>, GetUniverseQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetGenerationQuery, PatternView>, GetGenerationQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetFinalStateQuery, FinalStateView>, GetFinalStateQueryHandler>();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// The console exporter prints every span and every metric to stdout. That is useful when the console
// is a developer's terminal and indefensible when it is a log pipeline billed by volume, so it is
// configuration-driven and off outside Development. Swapping in OTLP is an exporter change and
// nothing more, which is the point of instrumenting against OpenTelemetry rather than a vendor SDK
// (README.md §9).
var useConsoleExporter = builder.Configuration.GetValue("Telemetry:ConsoleExporter", false);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(GameOfLifeDiagnostics.Name))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(GameOfLifeDiagnostics.Name);

        if (useConsoleExporter)
        {
            tracing.AddConsoleExporter();
        }
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter(GameOfLifeDiagnostics.Name)
            .AddMeter("Microsoft.AspNetCore.RateLimiting");

        if (useConsoleExporter)
        {
            metrics.AddConsoleExporter();
        }
    });

// The one admission control in the process: it bounds how many CPU-bound evaluations run at once,
// which the input caps in GameOfLifeOptions cannot do (those bound the cost of a single request).
// QueueLimit 0 rejects immediately rather than queueing, because a request answered after the client
// has given up has cost a core for nothing. See README.md §8.4.
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
                Detail = "Too many evaluations are in progress. Retry shortly."
            },
            options: null,
            contentType: "application/problem+json",
            ct);
    };
});

var app = builder.Build();

app.Services.EnsureDatabaseCreated();

// Request logging is the one line per request that matters, so its level is chosen rather than fixed.
// Health probes run every few seconds forever and say nothing when they pass, so they drop below the
// floor in every environment; a 4xx is the caller's problem and a 5xx is ours.
app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, exception) => exception is not null || httpContext.Response.StatusCode >= 500
        ? LogEventLevel.Error
        : httpContext.Response.StatusCode >= 400
            ? LogEventLevel.Warning
            : httpContext.Request.Path.StartsWithSegments("/health")
                ? LogEventLevel.Verbose
                : LogEventLevel.Information;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

// Off by default because the deployed topology terminates TLS at the ingress. With no HTTPS port to
// find, this middleware redirects nothing and logs a warning for every request that passes through it.
if (app.Configuration.GetValue("HttpsRedirection:Enabled", false))
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

/// <summary>
///     Entry point partial class, exposed so <c>WebApplicationFactory&lt;Program&gt;</c> can bootstrap it in
///     functional tests.
/// </summary>
public partial class Program
{
}
