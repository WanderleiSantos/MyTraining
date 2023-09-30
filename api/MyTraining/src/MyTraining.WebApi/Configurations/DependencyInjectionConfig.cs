using FluentValidation;
using MyTraining.API.Extensions;
using MyTraining.Application.Shared.Configurations;
using MyTraining.Application.UseCases.InsertUser;
using MyTraining.Application.UseCases.InsertUser.Commands;
using MyTraining.Application.UseCases.InsertUser.Validations;
using MyTraining.Application.UseCases.SignIn;
using MyTraining.Application.UseCases.SignIn.Commands;
using MyTraining.Application.UseCases.SignIn.Services;
using MyTraining.Application.UseCases.SignIn.Validations;
using MyTraining.Core.Interfaces;
using MyTraining.Core.Interfaces.Extensions;
using MyTraining.Core.Interfaces.Persistence.Repositories;
using MyTraining.Infrastructure.Persistence;
using MyTraining.Infrastructure.Persistence.Repositories;

namespace MyTraining.API.Configurations;

public static class DependencyInjectionConfig
{
    private const string JwtConfigurationSection = "JwtConfiguration";
    
    public static void AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
            
        services.AddScoped<DefaultDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IValidator<InsertUserCommand>, InsertUserCommandValidator>();
        services.AddScoped<IValidator<SignInCommand>, SignInCommandValidator>();
        
        services.AddScoped<IInsertUserUseCase, InsertUserUseCase>();
        services.AddScoped<ISignInUseCase, SignInUseCase>();
        
        services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfigurationSection));
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<ICurrentUser, CurrentUser>();
    }
}