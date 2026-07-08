using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "SpeededERP.Gateway")
    .WriteTo.Console());

// ---- Reverse proxy (architecture section 04/30) -----------------------------------
// A single cluster today (the modular monolith, SpeededERP.Api) — this is the
// deliberate extraction seam: when a module graduates to its own microservice
// (section 32, Strangler Fig), only this file's routes/clusters change, never the
// client apps that talk to the Gateway.
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();
