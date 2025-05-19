using System.Linq.Expressions;
using Framework.Application.Interfaces;

namespace Framework.Infrastructure.Logger;

public class LogSchemaPropertySelector : ISchemaPropertySelector
{
    private readonly Dictionary<Type, LambdaExpression> _selectors;

    public LogSchemaPropertySelector(ISchemaRegister selector)
    {
        _selectors = new();
        selector.Register(this);
    }

    public void Register<T>(Expression<Func<T, object>> selector)
    {
        _selectors[typeof(T)] = selector;
    }

    public object Get<T>(T request)
    {
        if (request == null)
            return null;

        if (_selectors.TryGetValue(request.GetType(), out var selector))
        {
            return selector.Compile().DynamicInvoke(request);
        }

        return null;
    }

}
