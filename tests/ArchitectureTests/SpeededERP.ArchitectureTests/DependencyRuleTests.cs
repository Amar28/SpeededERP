using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace SpeededERP.ArchitectureTests;

/// <summary>
/// Enforces the dependency rules stated in architecture section 06:
/// "Presentation → Application → Domain", "Infrastructure → Application + Domain",
/// "aucun module (Core inclus) ne référence l'assembly interne d'un autre module" —
/// and section 31's "Domain: aucune dépendance externe". These tests are the
/// automated guardrail so the rules survive beyond code review.
/// </summary>
public sealed class DependencyRuleTests
{
    public static IEnumerable<object[]> Modules => ModuleCatalog.AsTheoryData();

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_layer_does_not_depend_on_Application_Infrastructure_or_Presentation(ModuleAssemblies module)
    {
        var result = Types.InAssembly(module.Domain)
            .Should()
            .NotHaveDependencyOnAny(
                module.Application.GetName().Name!,
                module.Infrastructure.GetName().Name!,
                module.Presentation.GetName().Name!)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(module.ModuleName, "Domain", result));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_layer_has_no_dependency_on_frameworks(ModuleAssemblies module)
    {
        // Architecture section 31: "Domain — aucune dépendance externe (hors
        // Ardalis.GuardClauses optionnel)".
        var result = Types.InAssembly(module.Domain)
            .Should()
            .NotHaveDependencyOnAny(
                "MediatR",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "MassTransit",
                "FluentValidation")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(module.ModuleName, "Domain (frameworks)", result));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Application_layer_does_not_depend_on_Infrastructure_or_Presentation(ModuleAssemblies module)
    {
        var result = Types.InAssembly(module.Application)
            .Should()
            .NotHaveDependencyOnAny(module.Infrastructure.GetName().Name!, module.Presentation.GetName().Name!)
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(module.ModuleName, "Application", result));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Application_layer_has_no_dependency_on_ef_core_or_aspnetcore(ModuleAssemblies module)
    {
        var result = Types.InAssembly(module.Application)
            .Should()
            .NotHaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(BuildFailureMessage(module.ModuleName, "Application (frameworks)", result));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void No_module_references_another_modules_assembly(ModuleAssemblies module)
    {
        var otherModulesAssemblyNames = ModuleCatalog.All
            .Where(m => m.ModuleName != module.ModuleName)
            .SelectMany(m => new[]
            {
                m.Domain.GetName().Name!,
                m.Application.GetName().Name!,
                m.Infrastructure.GetName().Name!,
                m.Presentation.GetName().Name!,
            })
            .ToArray();

        foreach (var layer in new[] { module.Domain, module.Application, module.Infrastructure, module.Presentation })
        {
            var result = Types.InAssembly(layer)
                .Should()
                .NotHaveDependencyOnAny(otherModulesAssemblyNames)
                .GetResult();

            result.IsSuccessful.Should().BeTrue(BuildFailureMessage(module.ModuleName, $"{layer.GetName().Name} (cross-module isolation)", result));
        }
    }

    private static string BuildFailureMessage(string moduleName, string layer, TestResult result) =>
        $"{moduleName}.{layer} violates the dependency rule. Offending types: " +
        string.Join(", ", result.FailingTypeNames ?? []);
}
