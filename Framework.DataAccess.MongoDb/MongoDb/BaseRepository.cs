using System.Linq.Expressions;
using Framework.Application.Interfaces;
using Framework.Domain.Entities;
using Framework.Domain.Messages;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Framework.DataAccess.MongoDb.MongoDb;

public abstract class BaseRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
{
    protected readonly IDbContext Context;

    protected IQueryable<TEntity> Queryable => Context.GetQueryable<TEntity, TKey>().Data;

    protected BaseRepository(IDbContext context)
    {
        Context = context;
    }

    public virtual async Task<Result> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context
          .AddAsync<TEntity,TKey>(entity, cancellationToken: cancellationToken);


        return Result.Ok();
    }

    public virtual async Task<Result> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var exists = Context.GetQueryable<TEntity, TKey>(cancellationToken)
            .Data.Any(z => z.Id.Equals(entity.Id));

        if (exists)
            await UpdateAsync(entity, cancellationToken);
        else
            await AddAsync(entity, cancellationToken);

        return Result.Ok();
    }

    public virtual async Task<Result> AddOrUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        var existingEntity = Queryable.FirstOrDefault(condition);
        if (existingEntity == null)
            await AddAsync(entity, cancellationToken);
        else
        {
            entity.Id = existingEntity.Id;
            await UpdateAsync(entity, cancellationToken);
        }

        return Result.Ok();
    }

    public virtual async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var data = await Queryable.ToListAsync(cancellationToken);
        return data;
    }

    public virtual async Task<Result<TEntity>> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var items = await Queryable.Where(z => z.Id.Equals(id)).ToListAsync(cancellationToken);

        if (items is null || !items.Any())
            return GeneralMessages.NotFoundById().WithParams(id, typeof(TEntity).Name);

        if (items.Count > 1)
            return GeneralMessages.DuplicateFoundById.WithParams(id, typeof(TEntity).Name);

        return items.Single();
    }

    public virtual async Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default, bool isUpsert = default)
    {
        await Context.UpdateAsync<TEntity, TKey>(entity, isUpsert, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> UpdateManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Context.UpdateManyAsync<TEntity, TKey>(entities, cancellationToken);

        return Result.Ok();
    }

    public virtual async Task<Result> UpdateManyAsync<TProperty>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> property, CancellationToken cancellationToken = default)
    {
        await Context.UpdateManyAsync<TEntity,TProperty, TKey>(entities, property);

        return Result.Ok();
    }

    public virtual async Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.DeleteAsync<TEntity, TKey>(entity, cancellationToken);


        return Result.Ok();
    }

    public async Task<Result> AddManyAsync(List<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null || !entities.Any())
            return Result.Ok();

        await Context
            .AddManyAsync<TEntity, TKey>(entities, cancellationToken: cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> DeleteManyAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null || !entities.Any())
            return Result.Ok();

        await Context.DeleteManyAsync<TEntity, TKey>(entities, cancellationToken);


        return Result.Ok();
    }

    public async Task<Result> DeleteManyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        await Context.DeleteManyAsync<TEntity, TKey>(filter, cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> DeleteManyByIdAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        if (ids == null || !ids.Any())
            return Result.Ok();

        await Context.DeleteManyByIdAsync<TEntity, TKey>(ids, cancellationToken);


        return Result.Ok();
    }

    public async Task<Result<IReadOnlyList<TEntity>>> GetAllAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        if (condition == null)
            return GeneralMessages.ConditionCanNotBeNull;

        var data = await Queryable.Where(condition).ToListAsync(cancellationToken);

        return data;
    }

    public async Task<Result<IReadOnlyList<TKey>>> GetAllIdsAsync(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
    {
        if (condition == null)
            return GeneralMessages.ConditionCanNotBeNull;

        var data = await Queryable.Where(condition).Select(x => x.Id).ToListAsync(cancellationToken);

        return data;
    }

    public async Task<Result<IReadOnlyList<TResult>>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
    {
        if (condition == null)
            return GeneralMessages.ConditionCanNotBeNull;

        var data = await Queryable.Where(condition).Select(selector).ToListAsync(cancellationToken);

        return data;
    }

    public async Task<Result<bool>> IsMatchedByConditionAsync(Expression<Func<TEntity, bool>> matchCondition, CancellationToken cancellationToken = default)
    {
        var queryable = Context.GetQueryable<TEntity, TKey>(cancellationToken);
        var isMatched = Queryable.Where(matchCondition);
        var result = await isMatched.AnyAsync(cancellationToken);
        return result;
    }

}


