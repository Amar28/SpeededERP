using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SpeededERP.Modules.Core.Config.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations add` build this module's DbContext directly, without
/// spinning up SpeededERP.Api's full ASP.NET Core host as --startup-project (which
/// requires the exact ASP.NET Core shared framework the design-time tool was built
/// against). Design-time only — never used at runtime, where DependencyInjection.cs's
/// AddDbContext wires the real connection string from configuration.
/// </summary>
public sealed class ConfigDbContextFactory : IDesignTimeDbContextFactory<ConfigDbContext>
{
    public ConfigDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=speeded_erp;Username=speeded_erp;Password=changeme",
            npgsql => npgsql.MigrationsAssembly(typeof(ConfigDbContextFactory).Assembly.FullName));

        return new ConfigDbContext(optionsBuilder.Options);
    }
}
