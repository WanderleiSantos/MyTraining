using Application;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using WebApi;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ConfigureServices
builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure
app.ExecuteMigrations();
// app.UseHttpsRedirection();
app.UseCompressionSetup();
app.UseAuthenticationSetup();
app.MapControllers();
if (app.Environment.IsDevelopment())
    app.UseSwaggerSetup(app.Services.GetRequiredService<IApiVersionDescriptionProvider>());
app.Run();

public abstract partial class Program { }