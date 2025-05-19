using System.Reflection;
using Framework.WebApi.Settings;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.WebApi.SwaggerConfigs;

public class AddConstantsDocumentFilter : IDocumentFilter
{
    private readonly ApiConfigurationOption _option;

    public AddConstantsDocumentFilter(ApiConfigurationOption option)
    {
        _option = option;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var constantType in _option.ConstantTypes ?? [])
        {
            var schema = GenerateSchema(constantType);
            swaggerDoc.Components.Schemas[constantType.Name] = schema;
        }
    }

    private OpenApiSchema GenerateSchema(Type constantType)
    {
        var properties = new Dictionary<string, OpenApiSchema>();

        // Get all public constants from the constantType class
        var constants = constantType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(field => field.IsLiteral && !field.IsInitOnly); // Only const fields

        foreach (var constant in constants)
        {
            var constantName = constant.Name;
            var constantValue = constant.GetRawConstantValue();
            properties[constantName] = new OpenApiSchema
            {
                Type = "string",
                Enum = new List<IOpenApiAny> { new OpenApiString(constantValue.ToString()) }
            };
        }

        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = properties
        };
        return schema;
    }
}
