using Framework.WebApi.Settings;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.WebApi.SwaggerConfigs;

public class AdditionalDtoDocumentFilter : IDocumentFilter
{
    private readonly ApiConfigurationOption _option;

    public AdditionalDtoDocumentFilter(ApiConfigurationOption option)
    {
        _option = option;
    }
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var additionalDtoSchemas = new Dictionary<string, OpenApiSchema>();
        _option.AdditionalSwaggerSchemaTypes?.ToList().ForEach(z=> additionalDtoSchemas.Add(z.Name, context.SchemaGenerator.GenerateSchema(z, context.SchemaRepository)));
        
        foreach (var dtoSchema in additionalDtoSchemas)
        {
            if (!swaggerDoc.Components.Schemas.ContainsKey(dtoSchema.Key))
                swaggerDoc.Components.Schemas.Add(dtoSchema.Key, dtoSchema.Value);
        }
    }

}
