using Application.Shared.Configurations;
using Application.Shared.Services;
using Application.UseCases.Auth.SignIn;
using Application.UseCases.Auth.SignIn.Commands;
using Application.UseCases.Auth.SignIn.Validations;
using Application.UseCases.Exercises.InsertExercise;
using Application.UseCases.Exercises.InsertExercise.Commands;
using Application.UseCases.Exercises.InsertExercise.Validations;
using Application.UseCases.Exercises.SearchAllExercises;
using Application.UseCases.Exercises.SearchAllExercises.Commands;
using Application.UseCases.Exercises.SearchAllExercises.Validations;
using Application.UseCases.Exercises.SearchExerciseById;
using Application.UseCases.Exercises.SearchExerciseById.Commands;
using Application.UseCases.Exercises.SearchExerciseById.Validations;
using Application.UseCases.Exercises.UpdateExercise;
using Application.UseCases.Exercises.UpdateExercise.Commands;
using Application.UseCases.Exercises.UpdateExercise.Validations;
using Application.UseCases.Users.InsertUser;
using Application.UseCases.Users.InsertUser.Commands;
using Application.UseCases.Users.InsertUser.Validations;
using Application.UseCases.Users.SearchUserById;
using Application.UseCases.Users.SearchUserById.Commands;
using Application.UseCases.Users.SearchUserById.Validations;
using Application.UseCases.Users.UpdateUser;
using Application.UseCases.Users.UpdateUser.Commands;
using Application.UseCases.Users.UpdateUser.Validations;
using Core.Interfaces.Extensions;
using Core.Interfaces.Persistence.Repositories;
using FluentValidation;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using WebApi.Extensions;

namespace WebApi.Configurations;

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
        services.AddScoped<IValidator<SearchUserByIdCommand>, SearchUserByIdCommandValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
        services.AddScoped<IValidator<SignInCommand>, SignInCommandValidator>();
        services.AddScoped<IValidator<InsertExerciseCommand>, InsertExerciseCommandValidator>();
        services.AddScoped<IValidator<SearchExerciseByIdCommand>, SearchExerciseByIdValidator>();
        services.AddScoped<IValidator<SearchAllExercisesCommand>, SearchAllExercisesValidator>();
        services.AddScoped<IValidator<UpdateExerciseCommand>, UpdateExerciseValidator>();
        
        services.AddScoped<IInsertUserUseCase, InsertUserUseCase>();
        services.AddScoped<ISearchUserByIdUseCase, SearchUserByIdUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<ISignInUseCase, SignInUseCase>();
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