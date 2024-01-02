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

        services.AddHttpContextAccessor();
        
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}