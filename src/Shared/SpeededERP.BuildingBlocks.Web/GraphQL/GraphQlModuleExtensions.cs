using System.Reflection;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types;

namespace SpeededERP.BuildingBlocks.Web.GraphQL;

public static class GraphQlModuleExtensions
{
    /// <summary>
    /// Scans the given module Presentation assemblies for
    /// <c>[ExtendObjectType(OperationTypeNames.Query/Mutation)]</c> classes and
    /// registers them as type extensions on the shared schema — so adding a new
    /// module never requires touching SpeededERP.Api's GraphQL wiring by hand.
    /// </summary>
    public static IRequestExecutorBuilder AddModuleTypeExtensions(
        this IRequestExecutorBuilder builder,
        IEnumerable<Assembly> moduleAssemblies)
    {
        foreach (var assembly in moduleAssemblies.Distinct())
        {
            foreach (var type in assembly.GetExportedTypes())
            {
                if (type.GetCustomAttribute<ExtendObjectTypeAttribute>() is not null)
                {
                    builder.AddTypeExtension(type);
                }
            }
        }

        return builder;
    }
}
