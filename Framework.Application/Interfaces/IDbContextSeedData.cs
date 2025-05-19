using Framework.Domain.Entities;

namespace Framework.Application.Interfaces;

public interface IDbContextSeedData
{
    Task<Result> SeedDataAsync(CancellationToken cancellationToken = default);
}
