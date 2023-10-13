using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Extensions;

public static class MigrationExtensions
{
    public static IApplicationBuilder ExecuteMigrations(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<DefaultDbContext>();    
        context.Database.Migrate();

        return app;
    }
}