using System.Text.Json.Serialization;
using FluentValidation;
using Framework.Application.Interfaces;
using Framework.Application.Services;
using Framework.Infrastructure.Logger;
using Framework.WebApi.Authorizations;
using Framework.WebApi.Contracts;
using Framework.WebApi.Settings;
using Framework.WebApi.SwaggerConfigs;
using Framework.WebApi.Validators;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Framework.WebApi;

public static class WebApiDependencyInjection
{
    public static IServiceCollection AddWebApi(this IServiceCollection services, IConfiguration configuration, Action<ApiConfigurationOption>? setOptions = default)
    {
        var options = new ApiConfigurationOption();
        setOptions?.Invoke(options);

        services.AddScoped<IApplicationScopeInfo, ApplicationScopeInfo>();

        options.SetSetting(configuration?.GetSection(nameof(ProjectSettings))?.Get<ProjectSettings>() ?? new());

        services.AddTransient(z => options);

        services.AddMappingConfigs(options);
       
        services.AddControllers(opt =>
        {
            opt.Filters.Add<ValidationFilter>();
            opt.Conventions.Add(new RestApiConvention());
        }).AddJsonOptions(opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddEndpointsApiExplorer();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });
        });

        services.AddApplicationInsightsTelemetry();
        services.AddApplicationInsightsTelemetryProcessor<TelemetryProcessor>();
        services.AddFluentValidator(options);

        services.AddSingleton<ISchemaPropertySelector, LogSchemaPropertySelector>();
        services.AddSingleton<ISchemaRegister, DefaultLogSchema>();

        services.AddSwaggerConfigs(options);

        var authSettings = configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>();
        services.AddAuthenticationConfigs(authSettings, configuration);
        return services;
    }

    public static IServiceCollection AddCustomSchemaLoggerConfigs(this IServiceCollection services, ISchemaRegister schemaRegister)
    {
        services.AddSingleton(schemaRegister);
        return services;
    }

    private static IServiceCollection AddFluentValidator(this IServiceCollection services, ApiConfigurationOption option)
    {
        option.ValidatorsTypes?.ToList().ForEach(z => services.AddValidatorsFromAssemblyContaining(z));

        services.AddTransient<IAppValidator, FluentValidator>();
        return services;
    }

    private static IServiceCollection AddSwaggerConfigs(this IServiceCollection services, ApiConfigurationOption option)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = option.ProjectSettings.Name,
                Version = "1",
                License = new OpenApiLicense
                {
                    Name = "Vistex",
                    Url = new Uri("https://www.vistex.com")
                }
            });
            
            c.SwaggerDoc("internal", new OpenApiInfo
            {
                Title = option.ProjectSettings.Name + " internal",
                Version = "1",
                License = new OpenApiLicense
                {
                    Name = "Vistex",
                    Url = new Uri("https://www.vistex.com")
                }
            });
            
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                var tags = apiDesc.ActionDescriptor.EndpointMetadata
                    .OfType<TagsAttribute>()
                    .SelectMany(attr => attr.Tags)
                    .ToList();

                return string.Equals(docName, "v1", StringComparison.OrdinalIgnoreCase) && !tags.Contains("internal", StringComparer.OrdinalIgnoreCase) 
                       || string.Equals(docName, "internal", StringComparison.OrdinalIgnoreCase) && tags.Contains("internal", StringComparer.OrdinalIgnoreCase);

            });
            var securityDefinition = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            };

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
              {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
              }
            };

            if (option.TemplateIdIsRequired)
                c.OperationFilter<AddTemplateIdHeaderParameter>();

            c.AddSecurityRequirement(securityRequirement);
            c.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
            c.SchemaFilter<EnumSchemaFilter>();
            c.DocumentFilter<AdditionalDtoDocumentFilter>();
            c.DocumentFilter<AddConstantsDocumentFilter>();
            c.SupportNonNullableReferenceTypes();
            c.UseAllOfToExtendReferenceSchemas();
            c.UseAllOfForInheritance();
        });


        return services;
    }

    private static IServiceCollection AddAuthenticationConfigs(this IServiceCollection services, IdentitySettings settings, IConfiguration configuration)
    {
        bool.TryParse(configuration.GetSection(nameof(DevelopmentSettings.IgnoreTokenValidity)).Value, out bool ignoreTokenValidity);
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {

                options.Authority = settings.Authority;
                options.Audience = $"{settings.Authority}/resources";
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = !ignoreTokenValidity,
                    ValidateIssuerSigningKey = false,
                    ClockSkew = new TimeSpan(0, 0, 10),
                    SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                    {
                        var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);

                        return jwt;
                    },
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(MessageHub.HubRoute))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationsConsts.TenantIsRequired, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(AuthorizationsConsts.Tenant);
            });

        });
        return services;
    }

    private static IServiceCollection AddMappingConfigs(this IServiceCollection services, ApiConfigurationOption option)
    {
        TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
        var config = TypeAdapterConfig.GlobalSettings;
        var items = option.MappingTypes?.ToList() ?? new List<Type>();
        items.Add(typeof(ApiMessageResponse));
        config.Scan(items.Select(z => z.Assembly).ToArray());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}
