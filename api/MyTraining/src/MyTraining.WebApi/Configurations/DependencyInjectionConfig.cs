using FluentValidation;
using MyTraining.API.Extensions;
using MyTraining.Application.Shared.Configurations;
using MyTraining.Application.Shared.Services;
using MyTraining.Application.UseCases.Auth.SignIn;
using MyTraining.Application.UseCases.Auth.SignIn.Commands;
using MyTraining.Application.UseCases.Auth.SignIn.Validations;
using MyTraining.Application.UseCases.Exercises.InsertExercise;
using MyTraining.Application.UseCases.Exercises.InsertExercise.Commands;
using MyTraining.Application.UseCases.Exercises.InsertExercise.Validations;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Commands;
using MyTraining.Application.UseCases.Exercises.SearchAllExercises.Validations;
using MyTraining.Application.UseCases.Exercises.SearchExerciseById;
using MyTraining.Application.UseCases.Exercises.SearchExerciseById.Commands;
using MyTraining.Application.UseCases.Exercises.SearchExerciseById.Validations;
using MyTraining.Application.UseCases.Exercises.UpdateExercise;
using MyTraining.Application.UseCases.Exercises.UpdateExercise.Commands;
using MyTraining.Application.UseCases.Exercises.UpdateExercise.Validations;
using MyTraining.Application.UseCases.Users.InsertUser;
using MyTraining.Application.UseCases.Users.InsertUser.Commands;
using MyTraining.Application.UseCases.Users.InsertUser.Validations;
using MyTraining.Application.UseCases.Users.SearchUserById;
using MyTraining.Application.UseCases.Users.SearchUserById.Commands;
using MyTraining.Application.UseCases.Users.SearchUserById.Validations;
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
        services.AddScoped<IExerciseRepository, ExerciseRepository>();

        services.AddScoped<IValidator<InsertUserCommand>, InsertUserCommandValidator>();
        services.AddScoped<IValidator<SearchUserByIdCommand>, SearchUserByIdValidator>();
        services.AddScoped<IValidator<SignInCommand>, SignInCommandValidator>();
        services.AddScoped<IValidator<InsertExerciseCommand>, InsertExerciseCommandValidator>();
        services.AddScoped<IValidator<SearchExerciseByIdCommand>, SearchExerciseByIdValidator>();
        services.AddScoped<IValidator<SearchAllExercisesCommand>, SearchAllExercisesValidator>();
        services.AddScoped<IValidator<UpdateExerciseCommand>, UpdateExerciseValidator>();
        
        services.AddScoped<IInsertUserUseCase, InsertUserUseCase>();
        services.AddScoped<ISignInUseCase, SignInUseCase>();
        services.AddScoped<ISearchUserByIdUseCase, SearchUserByIdUseCase>();
        services.AddScoped<IInsertExerciseUseCase, InsertExerciseUseCase>();
        services.AddScoped<ISearchExerciseByIdUseCase, SearchExerciseByIdUseCase>();
        services.AddScoped<ISearchAllExercisesUseCase, SearchAllExercisesUseCase>();
        services.AddScoped<IUpdateExerciseUseCase, UpdateExerciseUseCase>();
        
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Configure<JwtConfiguration>(configuration.GetSection(JwtConfigurationSection));
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
    }
}