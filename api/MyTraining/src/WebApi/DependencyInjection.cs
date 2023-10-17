using System.Text.Json.Serialization;
using Application.Shared.Authentication;
using WebApi.Extensions;
using WebApi.Services;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        
        services.AddAuthorization();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
        services.AddEndpointsApiExplorer();
        services.AddApiVersioningConfig();
        services.AddCompression();
        services.AddSwagger();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        return services;
    }
}