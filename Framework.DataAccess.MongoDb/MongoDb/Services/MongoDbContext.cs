using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Framework.Domain.Entities;
using Framework.Domain.Messages;
using Framework.DataAccess.Databases.MongoDb.Configs;
using Framework.Application.Interfaces;
using Framework.DataAccess.MongoDb.MongoDb.Services;
using Framework.Domain.Attributes;

namespace Framework.DataAccess.Databases.MongoDb.Services;

public class MongoDbContext : IDbContext
{
    private readonly IMongoClient _mongoClient;
    private readonly string _dbName;
    private readonly IDbSessionManager _dbSessionManager;
    private readonly ILogger<MongoDbContext> _logger;

    public bool IgnoreAudits { get; set; } = false;

    public MongoDbContext(IMongoClient mongoClient, IMongoDbSerializer serializer, MongoDbSettings settings, IDbSessionManager dbSessionManager, ILogger<MongoDbContext> logger)
    {
        _mongoClient = mongoClient;
        _dbName = settings.Database;
        _dbSessionManager = dbSessionManager;
        _logger = logger;
        serializer.Register();

    }

    public async Task<Result> AddAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        await GetCollection<TEntity, TKey>().InsertOneAsync(_dbSessionManager.Session, entity, cancellationToken: cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> AddManyAsync<TEntity, TKey>(List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {

        await GetCollection<TEntity, TKey>().InsertManyAsync(_dbSessionManager.Session, entities, cancellationToken: cancellationToken);
        return Result.Ok();

    }

    public async Task<Result> DeleteAsync<TEntity, TKey>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        //TODO: version checking
        await GetCollection<TEntity, TKey>().DeleteOneAsync(_dbSessionManager.Session, z => z.Id.Equals(entity.Id), null, cancellationToken);
        return Result.Ok();

    }

    public async Task<Result> DeleteManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        await GetCollection<TEntity, TKey>().DeleteManyAsync(_dbSessionManager.Session, z => entities.Any(e => e.Id.Equals(z.Id)), null, cancellationToken);
        return Result.Ok();

    }

    public async Task<Result> DeleteManyAsync<TEntity, TKey>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        await GetCollection<TEntity, TKey>().DeleteManyAsync(_dbSessionManager.Session, filter, null, cancellationToken);
        return Result.Ok();

    }

    public async Task<Result> DeleteManyByIdAsync<TEntity, TKey>(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        await GetCollection<TEntity, TKey>().DeleteManyAsync(_dbSessionManager.Session, z => ids.Any(e => e.Equals(z.Id)), null, cancellationToken);
        return Result.Ok();

    }

    public Result<IQueryable<TEntity>> GetQueryable<TEntity, TKey>(CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        var queryableCollection = GetCollection<TEntity, TKey>().AsQueryable(_dbSessionManager.Session);
        return Result<IQueryable<TEntity>>.Ok(queryableCollection);
    }

    public async Task<Result> UpdateAsync<TEntity, TKey>(TEntity entity, bool isUpsert = default, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        //TODO: version checking

        var builder = new FilterDefinitionBuilder<TEntity>();
        var filter = builder.Eq(a => a.Id, entity.Id);
        await GetCollection<TEntity, TKey>().ReplaceOneAsync(_dbSessionManager.Session, filter, entity, new ReplaceOptions { IsUpsert = isUpsert }, cancellationToken: cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> UpdateManyAsync<TEntity, TKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        if (!entities.Any())
            return Result.Ok();

        var bulkUpdateOps = new List<WriteModel<TEntity>>();

        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);

            bulkUpdateOps.Add(new ReplaceOneModel<TEntity>(filter, entity));
        }

        await GetCollection<TEntity, TKey>().BulkWriteAsync(_dbSessionManager.Session, bulkUpdateOps, null, cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> UpdateManyAsync<TEntity, TProperty, TKey>(IEnumerable<TEntity> entities, Expression<Func<TEntity, TProperty>> property, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        if (!entities.Any())
            return Result.Ok();

        var bulkUpdateOps = new List<WriteModel<TEntity>>();

        foreach (var entity in entities)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            var compiledProperty = property.Compile();
            var fieldValue = compiledProperty(entity);

            var update = Builders<TEntity>.Update.Set(property, fieldValue);

            bulkUpdateOps.Add(new UpdateOneModel<TEntity>(filter, update));
        }

        await GetCollection<TEntity, TKey>().BulkWriteAsync(_dbSessionManager.Session, bulkUpdateOps, null, cancellationToken);
        return Result.Ok();
    }

    public IMongoCollection<T> GetCollection<T, TKey>() where T : BaseEntity<TKey>
    {
        var collectionAttr = typeof(T)
       .GetCustomAttributes(typeof(NameAttribute), true)
       .FirstOrDefault() as NameAttribute;

        var collectionName = collectionAttr?.Name ?? $"{typeof(T).Name.ToLower()}s" ;
        var collection = _mongoClient.GetDatabase(_dbName)
            .GetCollection<T>(collectionName);

        return collection;
    }

    public async Task<Result> DeleteAsync<TEntity, TKey>(Guid id, CancellationToken cancellationToken = default) where TEntity : BaseEntity<TKey>
    {
        await GetCollection<TEntity, TKey>().DeleteOneAsync(_dbSessionManager.Session, z => z.Id.Equals(id), null, cancellationToken);
        return Result.Ok();
    }


    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_dbSessionManager.Session.IsInTransaction)
                return Result.Ok();

            await _dbSessionManager.CommitTransactionAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            await _dbSessionManager.RollbackTransactionAsync(cancellationToken);
            _logger.LogError(ex, ex.Message);
            return ex.InnerException != null ? GeneralMessages.DbErrorWithInnerEx.WithParams(ex.Message, ex.InnerException.Message) : GeneralMessages.DbError.WithParams(ex.Message);
        }
    }

    public Result StartTransaction()
    {
        _dbSessionManager.StartTransaction();
        return Result.Ok();
    }
}
