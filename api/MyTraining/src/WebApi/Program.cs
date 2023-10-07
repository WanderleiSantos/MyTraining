using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WebApi.Configurations;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

// ConfigureServices
builder.Services.AddAuthenticationConfiguration(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddCompressionConfiguration();

builder.Services.AddWebApiConfiguration(builder.Configuration);
builder.Services.AddApplicationConfiguration(builder.Configuration);
builder.Services.AddInfrastructureConfiguration(builder.Configuration);

builder.Services.AddSwagger();

var app = builder.Build();

// Configure

// app.UseHttpsRedirection();

app.UseCompressionSetup();
app.UseAuthenticationSetup();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerSetup(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
}

app.Run();