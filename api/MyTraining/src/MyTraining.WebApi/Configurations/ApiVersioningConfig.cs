using Microsoft.AspNetCore.Mvc;

namespace MyTraining.API.Configurations;

public static class ApiVersioningConfig
{
    public static void AddApiVersioningConfiguration(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            // Retorna os headers "api-supported-versions" e "api-deprecated-versions"
            // indicando versões suportadas pela API e o que está como deprecated
            options.ReportApiVersions = true;

            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
        });

        services.AddVersionedApiExplorer(options =>
        {
            // Agrupar por número de versão
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // Necessário para o correto funcionamento das rotas
            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });
    }
}