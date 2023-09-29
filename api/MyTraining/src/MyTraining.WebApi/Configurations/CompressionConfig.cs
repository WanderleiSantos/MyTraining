using Microsoft.AspNetCore.ResponseCompression;

namespace MyTraining.API.Configurations;

public static class CompressionConfig
{
    public static void AddCompressionConfiguration(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
        });
    }

    public static void UseCompressionSetup(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        app.UseResponseCompression();
    }
}