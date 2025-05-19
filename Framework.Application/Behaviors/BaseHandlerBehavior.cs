using Framework.Application.Interfaces;
using Framework.Domain.Entities;
using Framework.Domain.Entities.Enums;
using Framework.Domain.Messages;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Framework.Application.Behaviors;

public partial class BaseHandlerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result, new()
{
    private readonly IDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly ISchemaPropertySelector _dataSelector;
    private readonly IAppValidator _appValidator;

    public BaseHandlerBehavior(IDbContext dbContext, ILogger<BaseHandlerBehavior<TRequest, TResponse>> logger, ISchemaPropertySelector dataSelector, IAppValidator appValidator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _dataSelector = dataSelector;
        _appValidator = appValidator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.General_RequestHandlingStarted(request.GetType().Name);
        var validationResult = await _appValidator.ValidateAsync(request)!;
        if (validationResult.HasError)
        {
            var result = new TResponse
            {
                Messages = validationResult.Messages!,
                Status = MessageType.Error
            };

            _logger.General_RequestGetsFailed(request.GetType().Name, _dataSelector.Get(request) ?? "-", result.Messages.Select(z => new { z.Text, z.Code }).ToList());
            return result;
        }

        var isCommand = request.GetType().Name.EndsWith("Command");

        if (isCommand)
        {
            _dbContext.StartTransaction();
        }

        var response = await next();

        if (response.HasError)
        {
            response.Status = MessageType.Error;
            _logger.General_RequestGetsFailed(request.GetType().Name, _dataSelector.Get(request) ?? "-", response.Messages.Select(z => new { z.Text, z.Code }).ToList());
            return response;
        }

        if (isCommand)
        {
            var transactionResult = await _dbContext.SaveChangesAsync(cancellationToken);

            if (transactionResult.HasError)
            {
                response.Messages = transactionResult.Messages;
                response.Status = MessageType.Error;
                _logger.General_RequestGetsFailed(request.GetType().Name, _dataSelector.Get(request) ?? "-", response.Messages.Select(z => new { z.Text, z.Code }).ToList());
                return response;
            }
        }

        _logger.General_RequestHandledSuccessfully(request.GetType().Name);
        return response;
    }
}
