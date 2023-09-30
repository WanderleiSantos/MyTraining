using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MyTraining.API.Configurations;

public static class ApiIdentityConfig
{
    public static void AddApiIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtConfiguration:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfiguration:Key"])),
                    ValidateLifetime = true,
                };
            });
    }

    public static void UseApiIdentitySetup(this IApplicationBuilder app)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));

        app.UseAuthentication();
        app.UseAuthorization();
    }
}