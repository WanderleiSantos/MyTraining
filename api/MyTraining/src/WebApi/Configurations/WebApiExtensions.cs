using Core.Interfaces.Services;
using WebApi.Services;

namespace WebApi.Configurations;

public static class WebApiExtensions
{
    public static IServiceCollection AddWebApiConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}