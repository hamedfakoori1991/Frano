using Framework.Domain.Entities;

namespace Framework.Application.Interfaces;

public interface IAppValidator
{
    Task<Result> ValidateAsync<T>(T request);
}
