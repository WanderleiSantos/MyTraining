using MyTraining.API.Extensions;
using MyTraining.Core.Interfaces;
using MyTraining.Infrastructure.Persistence;

namespace MyTraining.API.Configurations;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
            
        services.AddScoped<DefaultDbContext>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<ICurrentUser, CurrentUser>();
    }
}