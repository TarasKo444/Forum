using Forum.Application.Commands.Comment.Models;
using Forum.Application.Services;
using Forum.Domain.Entities;
using Mapster;
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