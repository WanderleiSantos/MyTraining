using Microsoft.EntityFrameworkCore;
using MyTraining.Infrastructure.Persistence;

namespace MyTraining.API.Configurations;

public static class DatabaseConfig
{
    public static void AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        var connectionString = configuration.GetConnectionString("DbContext");

        services.AddDbContext<DefaultDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}