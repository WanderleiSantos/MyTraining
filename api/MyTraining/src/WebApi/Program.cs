using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WebApi.Configurations;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ConfigureServices
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApiIdentityConfiguration(builder.Configuration);
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDependencyInjectionConfiguration(builder.Configuration);
builder.Services.AddCompressionConfiguration();
builder.Services.AddSwaggerConfiguration();

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

var app = builder.Build();

// Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseCompressionSetup();
app.UseApiIdentitySetup();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerSetup(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
}

app.Run();