using RabbitMQ.Client;

namespace SpeededERP.Api.Composition;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddSpeededErpHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();

        var defaultConnection = configuration.GetConnectionString("Default");
        if (!string.IsNullOrWhiteSpace(defaultConnection))
        {
            builder.AddNpgSql(defaultConnection, name: "postgresql", tags: ["ready"]);
        }

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            builder.AddRedis(redisConnection, name: "redis", tags: ["ready"]);
        }

        var rabbitHost = configuration["MessageBroker:Host"];
        if (!string.IsNullOrWhiteSpace(rabbitHost))
        {
            // RabbitMQ.Client 7.x (pulled in by MassTransit.RabbitMQ) dropped the
            // connection-string constructor in favor of ConnectionFactory + an async
            // connection factory delegate — the health check package's own
            // connection-string overload was compiled against RabbitMQ.Client 6.x's
            // now-removed IModel API and threw a TypeLoadException at runtime.
            var connectionFactory = new ConnectionFactory
            {
                HostName = rabbitHost,
                UserName = configuration["MessageBroker:Username"] ?? "guest",
                Password = configuration["MessageBroker:Password"] ?? "guest",
                VirtualHost = configuration["MessageBroker:VirtualHost"] ?? "/",
            };

            builder.AddRabbitMQ(
                factory: _ => connectionFactory.CreateConnectionAsync(),
                name: "rabbitmq",
                tags: ["ready"]);
        }

        return services;
    }
}
