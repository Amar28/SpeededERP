using System.Reflection;

namespace SpeededERP.ArchitectureTests;

/// <summary>
/// Every module's four layer assemblies, loaded by name — avoids needing a marker
/// type reference per layer per module (13 × 4 = 52) just to obtain an
/// <see cref="Assembly"/> instance for NetArchTest to inspect.
/// </summary>
public sealed record ModuleAssemblies(string ModuleName, Assembly Domain, Assembly Application, Assembly Infrastructure, Assembly Presentation);

public static class ModuleCatalog
{
    private static readonly string[] FullyQualifiedModuleNames =
    [
        "SpeededERP.Modules.Core.Identity",
        "SpeededERP.Modules.Core.Metadata",
        "SpeededERP.Modules.Core.Workflow",
        "SpeededERP.Modules.Core.Collaboration",
        "SpeededERP.Modules.Core.Documents",
        "SpeededERP.Modules.Core.Tasks",
        "SpeededERP.Modules.Core.Config",
        "SpeededERP.Modules.Business.CRM",
        "SpeededERP.Modules.Business.Comptabilite",
        "SpeededERP.Modules.Business.RH",
        "SpeededERP.Modules.Business.Stock",
        "SpeededERP.Modules.Business.Logistique",
        "SpeededERP.Modules.Business.Immobilier",
    ];

    public static IReadOnlyList<ModuleAssemblies> All { get; } = FullyQualifiedModuleNames
        .Select(name => new ModuleAssemblies(
            name,
            Assembly.Load($"{name}.Domain"),
            Assembly.Load($"{name}.Application"),
            Assembly.Load($"{name}.Infrastructure"),
            Assembly.Load($"{name}.Presentation")))
        .ToList();

    public static IEnumerable<object[]> AsTheoryData() => All.Select(m => new object[] { m });
}
