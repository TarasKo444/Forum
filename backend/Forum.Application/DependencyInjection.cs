using Microsoft.Extensions.DependencyInjection;

namespace Forum.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) 
    {
        services.AddMediator(options => 
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        
        services.AddScoped<IExternalAuthService, ExternalAuthService>();

        return services;
    }
}