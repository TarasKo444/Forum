using Forum.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Forum.IntegrationTests;

public class WebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
{
    public static Guid UserId = Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPolicyEvaluator>();
            services.AddSingleton<IPolicyEvaluator, DisableAuthenticationPolicyEvaluator>();

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ForumDbContext>));
            
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<ForumDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryForumTest");
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            using (var appContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>())
            {
                appContext.Database.EnsureDeleted();
                appContext.Database.EnsureCreated();
            }
        });
    }
}
