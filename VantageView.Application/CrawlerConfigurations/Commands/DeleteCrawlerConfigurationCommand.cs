using Framework.Domain.Entities;
using MapsterMapper;
using MediatR;
using VantageView.Application.Repositories;

namespace VantageView.Application.CrawlerConfigurations.Commands;
public record DeleteCrawlerConfigurationCommand(string Id) : IRequest<Result>;

public class DeleteCrawlerConfigurationCommandHandler : IRequestHandler<DeleteCrawlerConfigurationCommand, Result>
{
    private readonly ICrawlerConfigurationRepository _repository;

    public DeleteCrawlerConfigurationCommandHandler(ICrawlerConfigurationRepository repo)
    {
        _repository = repo;
    }
    public async Task<Result> Handle(DeleteCrawlerConfigurationCommand request, CancellationToken cancellationToken)
    {
        var config = await _repository.GetByIdAsync(request.Id, cancellationToken);
        await _repository.DeleteAsync(config.Data, cancellationToken);
        return Result.Ok();
    }
}
