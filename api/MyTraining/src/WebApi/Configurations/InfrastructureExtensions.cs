using Core.Interfaces.Persistence.Repositories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Configurations;

public static class InfrastructureExtensions
{
    private const string ConnectionStringSection = "DbContext";
    
    public static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services, IConfiguration configuration)
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