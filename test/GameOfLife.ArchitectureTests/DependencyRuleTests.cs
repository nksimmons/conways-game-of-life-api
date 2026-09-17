using NetArchTest.Rules;

namespace GameOfLife.ArchitectureTests;

/// <summary>
/// Asserts the dependency rule from AGENTS.md §1.1. If a project reference breaks the rule, these
/// tests fail; they are not to be relaxed or deleted to make a change compile.
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
        var result = Types.InCurrentDomain()
            .That().ResideInNamespace(DomainNamespace)
            .ShouldNot().HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Domain_should_not_depend_on_infrastructure_frameworks()
    {
        var result = Types.InCurrentDomain()
            .That().ResideInNamespace(DomainNamespace)
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
        var result = Types.InCurrentDomain()
            .That().ResideInNamespace(ApplicationNamespace)
            .ShouldNot().HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_should_not_depend_on_entity_framework_or_aspnetcore()
    {
        var result = Types.InCurrentDomain()
            .That().ResideInNamespace(ApplicationNamespace)
            .ShouldNot().HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_should_not_depend_on_application_or_api()
    {
        var result = Types.InCurrentDomain()
            .That().ResideInNamespace(InfrastructureNamespace)
            .ShouldNot().HaveDependencyOnAny(ApplicationNamespace, ApiNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result) =>
        result.IsSuccessful
            ? string.Empty
            : "Failing types: " + string.Join(", ", result.FailingTypeNames ?? Array.Empty<string>());
}
