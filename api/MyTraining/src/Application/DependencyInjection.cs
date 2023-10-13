using System.Globalization;
using System.Text;
using Application.Shared.Authentication;
using Application.UseCases.Auth.RefreshToken;
using Application.UseCases.Auth.RefreshToken.Commands;
using Application.UseCases.Auth.RefreshToken.Validations;
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
using Application.UseCases.TrainingSheets.InsertTrainingSheet;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Commands;
using Application.UseCases.TrainingSheets.InsertTrainingSheet.Validations;
using Application.UseCases.TrainingSheets.Services;
using Application.UseCases.Users.ChangeUserPassword;
using Application.UseCases.Users.ChangeUserPassword.Commands;
using Application.UseCases.Users.ChangeUserPassword.Validations;
using Application.UseCases.Users.InsertUser;
using Application.UseCases.Users.InsertUser.Commands;
using Application.UseCases.Users.InsertUser.Services;
using Application.UseCases.Users.InsertUser.Validations;
using Application.UseCases.Users.SearchUserById;
using Application.UseCases.Users.SearchUserById.Commands;
using Application.UseCases.Users.SearchUserById.Validations;
using Application.UseCases.Users.UpdateUser;
using Application.UseCases.Users.UpdateUser.Commands;
using Application.UseCases.Users.UpdateUser.Validations;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        services
            .AddAuth(configuration)
            .AddValidators()
            .AddUseCases()
            .AddScoped<IInitialLoadService, InitialLoadService>()
            .AddScoped<IDeactivateTrainingSheetService, DeactivateTrainingSheetService>();
        
        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<IInsertUserUseCase, InsertUserUseCase>();
        services.AddScoped<ISearchUserByIdUseCase, SearchUserByIdUseCase>();
        services.AddScoped<IUpdateUserUseCase, UpdateUserUseCase>();
        services.AddScoped<IChangeUserPasswordUseCase, ChangeUserPasswordUseCase>();
        services.AddScoped<ISignInUseCase, SignInUseCase>();
        services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        services.AddScoped<IInsertExerciseUseCase, InsertExerciseUseCase>();
        services.AddScoped<ISearchExerciseByIdUseCase, SearchExerciseByIdUseCase>();
        services.AddScoped<ISearchAllExercisesUseCase, SearchAllExercisesUseCase>();
        services.AddScoped<IUpdateExerciseUseCase, UpdateExerciseUseCase>();
        services.AddScoped<IInsertTrainingSheetUseCase, InsertTrainingSheetUseCase>();
        
        return services;
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
        
        services.AddScoped<IValidator<InsertUserCommand>, InsertUserCommandValidator>();
        services.AddScoped<IValidator<SearchUserByIdCommand>, SearchUserByIdCommandValidator>();
        services.AddScoped<IValidator<UpdateUserCommand>, UpdateUserCommandValidator>();
        services.AddScoped<IValidator<ChangeUserPasswordCommand>, ChangeUserPasswordCommandValidator>();
        services.AddScoped<IValidator<SignInCommand>, SignInCommandValidator>();
        services.AddScoped<IValidator<RefreshTokenCommand>, RefreshTokenCommandValidator>();
        services.AddScoped<IValidator<InsertExerciseCommand>, InsertExerciseCommandValidator>();
        services.AddScoped<IValidator<SearchExerciseByIdCommand>, SearchExerciseByIdValidator>();
        services.AddScoped<IValidator<SearchAllExercisesCommand>, SearchAllExercisesValidator>();
        services.AddScoped<IValidator<UpdateExerciseCommand>, UpdateExerciseValidator>();
        services.AddScoped<IValidator<InsertTrainingSheetCommand>, InsertTrainingSheetCommandValidator>();
        
        return services;
    }
    
    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));
        
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                };
            });
        
        return services;
    }
}