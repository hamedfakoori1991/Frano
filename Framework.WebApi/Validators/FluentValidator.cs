using FluentValidation;
using FluentValidation.Results;
using Framework.Application.Interfaces;
using Framework.Domain.Entities;
using Framework.Domain.Entities.Enums;
using Framework.Domain.Messages;
using Framework.WebApi.Settings;
using Microsoft.Extensions.Logging;

namespace Framework.WebApi.Validators;

public class FluentValidator : IAppValidator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger _logger;
    private readonly ApiConfigurationOption _apiConfigurationOption;

    public FluentValidator(IServiceProvider serviceProvider, ILogger<FluentValidator> logger, ApiConfigurationOption apiConfigurationOption)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _apiConfigurationOption = apiConfigurationOption;
    }

    public async Task<Result> ValidateAsync<T>(T request)
    {
        if (request == null)
            return Result.Ok();

        return await ValidateAsync(request.GetType(), request);
    }

    private async Task<Result> ValidateAsync(Type type, object request)
    {
        if (request == null || type == null || type.IsValueType || request is string)
            return Result.Ok();

        if (_apiConfigurationOption.ExcludedValidatorsTypes?.Contains(type) == true)
            return Result.Ok();

        var validator = FluentValidator.GetValidator(_serviceProvider, type);

        if (validator == null)
        {
           _logger.General_ThereIsNoValidatorForRequestedType(type.Name);
            return Result.Ok();
        }

        ValidationResult? validationResult = await validator.ValidateAsync((dynamic)request);

        if (!validationResult.IsValid)
            return GetErrors(validationResult.Errors);

        return Result.Ok();
    }
    private static dynamic? GetValidator(IServiceProvider serviceProvider, Type type)
    {
        Type genericType = typeof(IValidator<>).MakeGenericType(type);

        dynamic? validator = serviceProvider.GetService(genericType) as IValidator;

        return validator;
    }

    private static List<Message> GetErrors(List<ValidationFailure> failures)
    {
        return failures
            .Select(item => Message.CreateError(item.ErrorCode, item.ErrorMessage, "", ErrorType.Validation))
            .ToList();
    }

}
