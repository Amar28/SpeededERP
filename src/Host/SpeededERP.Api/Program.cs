using Microsoft.FeatureManagement;
using Serilog;
using SpeededERP.Api.Composition;
using SpeededERP.BuildingBlocks.Application;
using SpeededERP.BuildingBlocks.Infrastructure.Caching;
using SpeededERP.BuildingBlocks.Web;
using SpeededERP.BuildingBlocks.Web.Modules;
using SpeededERP.Modules.Core.Identity.Infrastructure.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.UseSpeededErpSerilog();

// ---- Cross-cutting building blocks ----------------------------------------------
builder.Services.AddSpeededErpWebCore();
builder.Services.AddSpeededErpApplicationBehaviors();
builder.Services.AddSpeededErpDistributedCache(builder.Configuration);
builder.Services.AddSpeededErpObservability(builder.Configuration);
builder.Services.AddSpeededErpHealthChecks(builder.Configuration);
builder.Services.AddSpeededErpApiVersioning();
builder.Services.AddSpeededErpOpenApi();
builder.Services.AddSpeededErpCors(builder.Configuration);
builder.Services.AddSpeededErpMessagingPublisherOnly(builder.Configuration);
builder.Services.AddFeatureManagement();
builder.Services.AddHttpClient();
builder.Services.AddProblemDetails();

// ---- Modules (architecture section 06) -------------------------------------------
var modules = ModuleCatalog.All;
builder.Services.AddModules(builder.Configuration, modules);

var moduleAssemblies = modules.Select(m => m.GetType().Assembly);
builder.Services.AddSpeededErpGraphQl(moduleAssemblies);

// Dev-only bootstrap: no other way exists yet to obtain a first authenticated
// account (see DevAdminSeeder). Registered here rather than from Worker's
// AddIdentityBackgroundServices because Api's DI graph (MediatR, ITenantContext,
// DataProtection) is fully wired via AddModules, unlike Worker's.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHostedService<DevAdminSeeder>();
}

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}
else
{
    app.UseExceptionHandler();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapModules(modules);
app.MapGraphQL();

app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

app.Run();

// Exposed for WebApplicationFactory-based integration tests (tests/IntegrationTests).
public partial class Program;
