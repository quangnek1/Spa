using Microsoft.Extensions.DependencyInjection;

namespace Spa.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ConfigureServices).Assembly);

        return services;
    }
}