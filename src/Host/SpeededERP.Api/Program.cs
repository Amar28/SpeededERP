using Microsoft.FeatureManagement;
using Serilog;
using SpeededERP.Api.Composition;
using SpeededERP.BuildingBlocks.Application;
using SpeededERP.BuildingBlocks.Web;
using SpeededERP.BuildingBlocks.Web.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.UseSpeededErpSerilog();

// ---- Cross-cutting building blocks ----------------------------------------------
builder.Services.AddSpeededErpWebCore();
builder.Services.AddSpeededErpApplicationBehaviors();
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
