using Asp.Versioning;
using SpeededERP.BuildingBlocks.Web.GraphQL;

namespace SpeededERP.Api.Composition;

/// <summary>Versioning, OpenAPI, GraphQL, feature flags, CORS — architecture sections 19, 29.</summary>
public static class ApiPlatformExtensions
{
    public static IServiceCollection AddSpeededErpApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }

    public static IServiceCollection AddSpeededErpOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(document =>
        {
            document.Title = "SpeededERP API";
            document.Description = "API REST modulaire — un groupe d'endpoints par module métier/Core (architecture section 19).";
            document.Version = "v1";
        });

        return services;
    }

    /// <summary>
    /// One shared GraphQL schema; every module contributes fields via
    /// [ExtendObjectType], discovered by assembly scan — see GraphQlModuleExtensions.
    /// </summary>
    public static IServiceCollection AddSpeededErpGraphQl(this IServiceCollection services, IEnumerable<System.Reflection.Assembly> moduleAssemblies)
    {
        services.AddAuthorization();

        services
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddModuleTypeExtensions(moduleAssemblies);

        return services;
    }

    public static IServiceCollection AddSpeededErpCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        services.AddCors(options => options.AddDefaultPolicy(policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
        }));

        return services;
    }
}
