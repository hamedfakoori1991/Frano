namespace Framework.WebApi.Settings;

public class ApiConfigurationOption
{
  
    public ApiConfigurationOption SetAdditionalOpenApiSchemas(params Type[] schemaTypes)
    {
        AdditionalSwaggerSchemaTypes = schemaTypes;
        return this;
    }
    public ApiConfigurationOption SetConstantIntoOpenApiSchemas(params Type[] schemaTypes)
    {
        ConstantTypes = schemaTypes;
        return this;
    }
    public ApiConfigurationOption AddMappingFromAssemblyContaining(params Type[] mappingTypes)
    {
        MappingTypes = mappingTypes;
        return this;
    }
    public ApiConfigurationOption AddValidatorsFromAssemblyContaining(params Type[] types)
    {
        ValidatorsTypes = types;
        return this;
    }

    public ApiConfigurationOption ExcludeValidatorsFromAutoValidation(params Type[] types)
    {
        ExcludedValidatorsTypes = types;
        return this;
    }

    public ApiConfigurationOption SetTemplateIdAsRequired()
    {
        TemplateIdIsRequired = true;
        return this;
    }

    public ApiConfigurationOption SetSetting(ProjectSettings projectSettings)
    {
        ProjectSettings = projectSettings;
        return this;
    }

    public bool TemplateIdIsRequired { get; private set; } = false;
    public Type[] MappingTypes { get; private set; } = null!;
    public Type[] ValidatorsTypes { get; private set; } = null!;
    public Type[] ExcludedValidatorsTypes { get; private set; } = null!;
    public Type[] AdditionalSwaggerSchemaTypes { get; private set; } = null!;
    public Type[] ConstantTypes { get; private set; } = null!;
    public ProjectSettings ProjectSettings { get; private set; } = new();
}
