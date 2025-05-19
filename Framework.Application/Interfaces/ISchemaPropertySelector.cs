using System.Linq.Expressions;

namespace Framework.Application.Interfaces;

public interface ISchemaPropertySelector
{
    void Register<T>(Expression<Func<T, object>> selector);
    object Get<T>(T request);
}
