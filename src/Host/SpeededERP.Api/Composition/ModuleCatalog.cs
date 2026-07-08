using SpeededERP.BuildingBlocks.Web.Modules;

namespace SpeededERP.Api.Composition;

/// <summary>
/// The single place that lists every Core/Business module loaded by this process
/// (architecture section 06: "SpeededERP.Api — Composition root — charge chaque
/// IModule"). Adding a module to the platform means adding one line here — nothing
/// else in Program.cs changes, since REST/GraphQL mapping and DI registration are both
/// driven generically off this list (see ModuleExtensions and GraphQlModuleExtensions).
/// </summary>
public static class ModuleCatalog
{
    public static IReadOnlyList<IModule> All { get; } =
    [
        // Core modules — always active, never disabled by a tenant (section 05).
        new SpeededERP.Modules.Core.Identity.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Core.Metadata.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Core.Workflow.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Core.Collaboration.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Core.Documents.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Core.Tasks.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Core.Config.Presentation.ModuleRegistration(),

        // Business modules — activatable per tenant (section 16/17).
        new SpeededERP.Modules.Business.CRM.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Business.Comptabilite.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Business.RH.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Business.Stock.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Business.Logistique.Presentation.ModuleRegistration(),
        new SpeededERP.Modules.Business.Immobilier.Presentation.ModuleRegistration(),
    ];
}
