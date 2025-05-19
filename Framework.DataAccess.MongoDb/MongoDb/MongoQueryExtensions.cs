using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.DataAccess.MongoDb.MongoDb;

public static class MongoQueryExtensions
{
    public const string MongoId = "_id";
    public const string Id = "id";

    public static string GetNameOfSelector(this LambdaExpression selector)
    {
        var memberExpression = selector.Body as MemberExpression;
        return memberExpression!.Member.Name.ToLowerCase();
    }

    public static string ToLowerCase(this string val) => char.ToLowerInvariant(val[0]) + val.Substring(1);

    public static PropertyInfo HasPropertyOfType<TInner>(Type outer)
    {
        var properties = outer.GetProperties();
        foreach (var property in properties)
        {
            if (property.PropertyType.UnderlyingSystemType == typeof(List<TInner>) || property.PropertyType.UnderlyingSystemType == typeof(TInner))
            {
                return property;
            }
        }
        return null;
    }

  
    public static IAggregateFluent<T> QueryUnwind<T>(this IAggregateFluent<T> aggregate, string @as, bool preserveNullAndEmptyArrays = true)
    {
        aggregate = aggregate
            .Unwind(@as, new AggregateUnwindOptions<T>()
            {
                PreserveNullAndEmptyArrays = preserveNullAndEmptyArrays
            });

        return aggregate;
    }
    public static IAggregateFluent<T> Where<T>(this IAggregateFluent<T> aggregator, Expression<Func<T, bool>>? expression)
    {
        if (expression == null)
            return aggregator;

        aggregator = aggregator.Match(expression);
        return aggregator;
    }

    public static IAggregateFluent<T> Select<T>(this IAggregateFluent<T> aggregator, Expression<Func<T, T>>? expression)
    {
        if (expression == null)
            return aggregator;

        aggregator = aggregator.Project(expression);
        return aggregator;
    }
}
