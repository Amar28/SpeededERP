using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Serilog;
using SpeededERP.BuildingBlocks.Application;
using SpeededERP.BuildingBlocks.Application.Security;
using SpeededERP.BuildingBlocks.Infrastructure.Caching;
using SpeededERP.Worker.Composition;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) => configuration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "SpeededERP.Worker")
    .WriteTo.Console());

// ---- Cross-cutting building blocks ----------------------------------------------
builder.Services.AddSpeededErpApplicationBehaviors();
builder.Services.AddSpeededErpDistributedCache(builder.Configuration);
builder.Services.AddSingleton<ICurrentUserContext, SystemCurrentUserContext>();
builder.Services.AddSingleton<ITenantContext, SystemTenantContext>();

// ---- Every module's DbContext/repositories + Outbox publisher + seeders ----------
builder.Services.AddAllModulesInfrastructure(builder.Configuration);

// ---- MassTransit / RabbitMQ: this is the ONE process that consumes -------------
builder.Services.AddMassTransit(bus =>
{
    bus.SetKebabCaseEndpointNameFormatter();

    bus.ConfigureAllModulesConsumers();

    bus.UsingRabbitMq((context, rabbitCfg) =>
    {
        rabbitCfg.Host(
            builder.Configuration["MessageBroker:Host"] ?? "localhost",
            builder.Configuration["MessageBroker:VirtualHost"] ?? "/",
            host =>
            {
                host.Username(builder.Configuration["MessageBroker:Username"] ?? "guest");
                host.Password(builder.Configuration["MessageBroker:Password"] ?? "guest");
            });

        rabbitCfg.UseMessageRetry(retry => retry.Exponential(
            retryLimit: 5,
            minInterval: TimeSpan.FromSeconds(1),
            maxInterval: TimeSpan.FromSeconds(30),
            intervalDelta: TimeSpan.FromSeconds(2)));

        rabbitCfg.ConfigureEndpoints(context);
    });
});

// ---- Hangfire: scheduled/background jobs (imports, exports, OCR, rapports) ------
// architecture section 02/24.
var defaultConnection = builder.Configuration.GetConnectionString("Default");
if (!string.IsNullOrWhiteSpace(defaultConnection))
{
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(defaultConnection)));

    builder.Services.AddHangfireServer();
}

var host = builder.Build();
host.Run();
