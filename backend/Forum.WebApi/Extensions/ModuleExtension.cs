namespace Forum.WebApi.Extensions;

public interface IModule
{
    IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
}
 
public static class ModuleExtensions
{
    static readonly List<IModule> registeredModules = new List<IModule>();
 
    public static IServiceCollection RegisterModules(this IServiceCollection services)
    {
        var modules = DiscoverModules();
        foreach (var module in modules)
        {
            registeredModules.Add(module);
        }
 
        return services;
    }
 
    public static RouteGroupBuilder MapEndpoints(this RouteGroupBuilder route)
    {
        foreach (var module in registeredModules)
        {
            module.MapEndpoints(route);
        }
        return route;
    }
 
    private static IEnumerable<IModule> DiscoverModules()
    {
        return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
    }
}
