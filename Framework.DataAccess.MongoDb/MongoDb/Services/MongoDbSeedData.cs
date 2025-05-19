using Framework.Application.Interfaces;
using Framework.Domain.Entities;

namespace Framework.DataAccess.Databases.MongoDb.Services;

public class MongoDbSeedData : IDbContextSeedData
{
    public MongoDbSeedData()
    {
    }
    public async Task<Result> SeedDataAsync(CancellationToken cancellationToken = default)
    {
        // seed required information
        return Result.Ok();
    }
}
