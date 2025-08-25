using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OnAim.Aggregation.Infrastructure;

public static class ApplicationExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITopicNameResolver, TopicNameResolver>();
        var pg = config.GetConnectionString("Postgres");
        var mq = config.GetSection("RabbitMQ");

        services.AddCap(x =>
        {
            x.UsePostgreSql(opt => opt.ConnectionString = pg);
            x.UseRabbitMQ(opt =>
            {
                opt.HostName = mq.GetValue<string>("HostName")!;
                opt.VirtualHost = mq.GetValue<string>("VirtualHost") ?? "/";
                opt.UserName = mq.GetValue<string>("UserName")!;
                opt.Password = mq.GetValue<string>("Password")!;
            });
            x.FailedRetryCount = 50;
            x.FailedThresholdCallback = failed =>
            {
                // optional: log or alert when moved to DLQ
                // failed.Message.GetName(), failed.Group, failed.Exception
            };
            x.UseDashboard();
        });

        return services;
    }
}