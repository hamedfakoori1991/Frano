
using Framework.Application.Interfaces;
using Framework.Domain.Entities;
using MediatR;

namespace Framework.Application.Behaviors;

public class BaseScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result, new()
{
    private readonly IApplicationScopeInfo _applicationScopeInfo;

    public BaseScopeBehavior(IApplicationScopeInfo applicationScopeInfo)
    {
        _applicationScopeInfo = applicationScopeInfo;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        return await next();
    }
}



