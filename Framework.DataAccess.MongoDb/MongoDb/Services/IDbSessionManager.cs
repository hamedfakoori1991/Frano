using MongoDB.Driver;

namespace Framework.DataAccess.MongoDb.MongoDb.Services;

public interface IDbSessionManager : IDisposable
{
    void StartTransaction();
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
    IClientSessionHandle Session { get; }
}
