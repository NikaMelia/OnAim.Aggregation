using OnAim.Aggregation.Api.Dtos.Mapping;

namespace OnAim.Aggregation.Api;

public static class ApiExtensions
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        // Program.cs
        services.AddAutoMapper(_ => { }, typeof(ConfigMappingProfile));
        return services;
    }
}