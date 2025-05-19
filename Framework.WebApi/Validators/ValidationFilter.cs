using Framework.Application.Interfaces;
using Framework.Domain.Messages;
using Framework.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Framework.WebApi.Validators;

/// <summary>
/// this attribute is responsible to check that an action method meets the validation requirements
/// </summary>
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IAppValidator _validator;
    private readonly ILogger _logger;
    private readonly ISchemaPropertySelector _dataSelector;

    public ValidationFilter(IAppValidator validator, ILogger<ValidationFilter> logger, ISchemaPropertySelector dataSelector)
    {
        _validator = validator;
        _logger = logger;
        _dataSelector = dataSelector;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var arg = context.ActionArguments.FirstOrDefault().Value;

        var validationResult = await _validator.ValidateAsync(arg);

        if (!validationResult.HasError)
        {
           _logger.General_ApiRequestValidatedSuccessfully(context.HttpContext.Request.Path, arg?.GetType().Name ?? string.Empty);
            await next();
        }
        else
        {
            _logger.General_ApiRequestGetsFailed(arg?.GetType()?.Name?? "-", _dataSelector.Get(arg) ?? "-", validationResult.Messages.Select(z => new { z.Text, z.Code }).ToList());

            context.Result = validationResult!.GetMessagesResult(_logger);
        }
    }
}

