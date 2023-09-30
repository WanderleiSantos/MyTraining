using FluentValidation;
using MyTraining.API.Extensions;
using MyTraining.Application.UseCases.InsertUser;
using MyTraining.Application.UseCases.InsertUser.Commands;
using MyTraining.Application.UseCases.InsertUser.Validations;
using MyTraining.Core.Interfaces;
using MyTraining.Core.Interfaces.Extensions;
using MyTraining.Core.Interfaces.Persistence.Repositories;
using MyTraining.Infrastructure.Persistence;
using MyTraining.Infrastructure.Persistence.Repositories;

namespace MyTraining.API.Configurations;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
            
        services.AddScoped<DefaultDbContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IValidator<InsertUserCommand>, InsertUserCommandValidator>();
        
        services.AddScoped<IInsertUserUseCase, InsertUserUseCase>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddScoped<ICurrentUser, CurrentUser>();
    }
}