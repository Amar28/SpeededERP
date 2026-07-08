using MassTransit;

namespace SpeededERP.Api.Composition;

/// <summary>
/// Host.Api only ever publishes (e.g. module startup seeders) — it never consumes.
/// Consumers are exclusively registered in SpeededERP.Worker so exactly one process
/// competes for each queue (architecture section 06).
/// </summary>
public static class MessagingExtensions
{
    public static IServiceCollection AddSpeededErpMessagingPublisherOnly(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            bus.UsingRabbitMq((context, rabbitCfg) =>
            {
                rabbitCfg.Host(
                    configuration["MessageBroker:Host"] ?? "localhost",
                    configuration["MessageBroker:VirtualHost"] ?? "/",
                    host =>
                    {
                        host.Username(configuration["MessageBroker:Username"] ?? "guest");
                        host.Password(configuration["MessageBroker:Password"] ?? "guest");
                    });

                rabbitCfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
