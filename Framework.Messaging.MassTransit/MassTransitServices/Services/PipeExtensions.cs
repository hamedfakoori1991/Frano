using Framework.Application.Interfaces;
using Framework.Messaging.MassTransit.MassTransitServices.Filters;
using MassTransit;

namespace Framework.Messaging.MassTransit.MassTransitServices.Services;

public static class PipeExtensions
{
    public static IPipe<SendContext<T>> AddPipe<T>(this IApplicationScopeInfo scopeInfo) where T : class
    {
        return Pipe.New<SendContext<T>>(cfg => cfg.UseFilter(new AddHeaderFilter<T>(scopeInfo)));
    }
}

