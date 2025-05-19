using Framework.DataAccess.MongoDb.MongoDb.Services;
using MongoDB.Driver;

namespace Framework.DataAccess.Databases.MongoDb.Services;

public class MongoDbSessionManager : IDbSessionManager
{
    public IClientSessionHandle Session { get; }

    public MongoDbSessionManager(IMongoClient mongoClient)
    {
        Session = mongoClient.StartSession();
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await Session.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        await Session.AbortTransactionAsync(cancellationToken);
    }

    public void StartTransaction()
    {
        if (Session.IsInTransaction)
        {
            throw new Exception("DbError: Attempted to start a transaction when one was already open");
        }
        ;

        Session.StartTransaction();
    }

    public void Dispose()
    {
        Session?.Dispose();
    }
}
