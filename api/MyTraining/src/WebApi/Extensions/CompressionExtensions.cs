using Microsoft.AspNetCore.ResponseCompression;

namespace WebApi.Extensions;

public static class CompressionExtensions
{
    public static IServiceCollection AddCompression(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });

        return services;
    }

    public static void UseCompressionSetup(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        app.UseResponseCompression();
    }
}