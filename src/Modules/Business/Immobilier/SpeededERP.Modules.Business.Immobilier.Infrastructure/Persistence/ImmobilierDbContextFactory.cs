using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SpeededERP.Modules.Business.Immobilier.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations add` build this module's DbContext directly, without
/// spinning up SpeededERP.Api's full ASP.NET Core host as --startup-project (which
/// requires the exact ASP.NET Core shared framework the design-time tool was built
/// against). Design-time only — never used at runtime, where DependencyInjection.cs's
/// AddDbContext wires the real connection string from configuration.
/// </summary>
public sealed class ImmobilierDbContextFactory : IDesignTimeDbContextFactory<ImmobilierDbContext>
{
    public ImmobilierDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ImmobilierDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=speeded_erp;Username=speeded_erp;Password=changeme",
            npgsql => npgsql.MigrationsAssembly(typeof(ImmobilierDbContextFactory).Assembly.FullName));

        return new ImmobilierDbContext(optionsBuilder.Options);
    }
}
