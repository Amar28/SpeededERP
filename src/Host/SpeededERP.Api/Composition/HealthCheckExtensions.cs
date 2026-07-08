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
            var user = configuration["MessageBroker:Username"] ?? "guest";
            var password = configuration["MessageBroker:Password"] ?? "guest";
            var vhost = configuration["MessageBroker:VirtualHost"] ?? "/";
            builder.AddRabbitMQ(
                rabbitConnectionString: $"amqp://{user}:{password}@{rabbitHost}{vhost}",
                name: "rabbitmq",
                tags: ["ready"]);
        }

        return services;
    }
}
