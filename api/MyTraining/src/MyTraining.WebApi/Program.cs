using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using MyTraining.API.Configurations;

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
builder.Services.AddDependencyInjectionConfiguration();
builder.Services.AddCompressionConfiguration();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Configure
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCompressionSetup();
app.UseApiIdentitySetup();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerSetup(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
}

app.Run();