using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Framework.WebApi.SwaggerConfigs;

public class AddTemplateIdHeaderParameter : IOperationFilter
{
    public const string TemplateId = "Template-Id";
    public const string DocumentId = "Document-Id";
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }

        var hasExcludeTemplateIdHeaderAttribute = context.MethodInfo.GetCustomAttribute<DisableTemplateHeaderAttribute>() != null;

        if (!hasExcludeTemplateIdHeaderAttribute)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = TemplateId,
                In = ParameterLocation.Header,
                Description = "Template ID header",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = DocumentId,
                In = ParameterLocation.Header,
                Description = "Document ID header",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}

