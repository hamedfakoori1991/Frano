using System.Linq.Expressions;
using Framework.Domain.Entities;

namespace Framework.Application.Interfaces;
public interface IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    Task<Result> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> AddOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default);
    Task<Result> AddManyAsync(List<TEntity> entities, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TKey>>> GetAllIdsAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<TResult>>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default);
    Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default, bool isUpsert = default);
    Task<Result> UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<Result> UpdateManyAsync<TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> property, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<Result> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<Result> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    Task<Result> DeleteManyByIdAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<Result<bool>> IsMatchedByConditionAsync(Expression<Func<TEntity, bool>> matchCondition, CancellationToken cancellationToken = default);
}
