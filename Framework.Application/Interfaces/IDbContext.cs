using System.Linq.Expressions;
using Framework.Domain.Entities;

namespace Framework.Application.Interfaces;

public interface IDbContext
{
    Task<Result> AddAsync<TEntity,TKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> AddManyAsync<TEntity,TKey>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Result<IQueryable<TEntity>> GetQueryable<TEntity, TKey>(CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync<TEntity, TKey>(TEntity entity, bool isUpsert = default, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> UpdateManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> UpdateManyAsync<TEntity, TProperty,TKey>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> property, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> DeleteAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> DeleteAsync<TEntity, TKey>(Guid id, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> DeleteManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> DeleteManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Task<Result> DeleteManyByIdAsync<TEntity, TKey>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>;
    Result StartTransaction();
}
