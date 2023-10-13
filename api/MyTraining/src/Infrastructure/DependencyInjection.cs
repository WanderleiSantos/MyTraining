using Core.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string ConnectionStringSection = "DbContext";
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        
        var connectionString = configuration.GetConnectionString(ConnectionStringSection);

        services.AddDbContext<DefaultDbContext>(options => options.UseNpgsql(connectionString));
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        
        // services.AddDbContext<DefaultDbContext>(options => options.UseInMemoryDatabase("Default"));
        
        services.AddScoped<DefaultDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IExerciseRepository, ExerciseRepository>();
        services.AddScoped<ITrainingSheetRepository, TrainingSheetRepository>();
        
        return services;
    }
}