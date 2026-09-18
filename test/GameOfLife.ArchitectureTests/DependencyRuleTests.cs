using GameOfLife.Application.GetUniverse;
using GameOfLife.Domain.Domain;
using GameOfLife.Infrastructure.Persistence;
using NetArchTest.Rules;

namespace GameOfLife.ArchitectureTests;

/// <summary>
///     Asserts the dependency rule from AGENTS.md §1.1. If a project reference breaks the rule, these
///     tests fail; they are not to be relaxed or deleted to make a change compile.
/// </summary>
public sealed class DependencyRuleTests
{
    private const string DomainNamespace = "GameOfLife.Domain";
    private const string ApplicationNamespace = "GameOfLife.Application";
    private const string InfrastructureNamespace = "GameOfLife.Infrastructure";
    private const string ApiNamespace = "GameOfLife.Api";

    [Fact]
    public void Domain_should_not_depend_on_any_other_layer()
    {
        var result = ProductionTypes<Universe>()
            .ShouldNot().HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Domain_should_not_depend_on_infrastructure_frameworks()
    {
        var result = ProductionTypes<Universe>()
            .ShouldNot().HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "System.Data")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_should_only_depend_on_domain()
    {
        var result = ProductionTypes<GetUniverseQueryHandler>()
            .ShouldNot().HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_should_not_depend_on_entity_framework_or_aspnetcore()
    {
        var result = ProductionTypes<GetUniverseQueryHandler>()
            .ShouldNot().HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_should_not_depend_on_application_or_api()
    {
        var result = ProductionTypes<EfUniverseRepository>()
            .ShouldNot().HaveDependencyOnAny(ApplicationNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Dependency_check_detects_a_known_forbidden_reference()
    {
        var selection = Types.InAssembly(typeof(DependencyRuleTests).Assembly)
            .That().HaveName(nameof(DomainDependentFixture));
        Assert.Contains(typeof(DomainDependentFixture), selection.GetTypes());

        var result = selection.ShouldNot().HaveDependencyOnAny(DomainNamespace).GetResult();

        Assert.False(result.IsSuccessful);
    }

    private static Types ProductionTypes<T>()
    {
        var types = Types.InAssembly(typeof(T).Assembly);
        var selected = types.GetTypes().ToArray();
        Assert.NotEmpty(selected);
        Assert.Contains(typeof(T), selected);
        return types;
    }

    private sealed class DomainDependentFixture
    {
        public Universe? Universe { get; init; }
    }

    private static string Describe(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Failing types: " + string.Join(", ", result.FailingTypeNames ?? []);
}
