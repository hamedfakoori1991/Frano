using Framework.Application.Interfaces;
using Framework.Domain.Entities;
using MediatR;
using VantageView.Application.Repositories;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.Application.CrawlerConfigurations.Commands;
public record SetupANewCrawlerConfigurationCommand(string Title) : IRequest<Result<string>>;

public class SetupANewCrawlerConfigurationCommandHandler : IRequestHandler<SetupANewCrawlerConfigurationCommand, Result<string>>
{
    private readonly ICrawlerConfigurationRepository _repository;
    private readonly IdGenerator _idGenerator;

    public SetupANewCrawlerConfigurationCommandHandler(ICrawlerConfigurationRepository repo, IdGenerator idGenerator)
    {
        _repository = repo;
        _idGenerator = idGenerator;
    }
    public async Task<Result<string>> Handle(SetupANewCrawlerConfigurationCommand request, CancellationToken cancellationToken)
    {
        var config = new CrawlerConfiguration
        {
            Id = _idGenerator.NewId(),
            Title = request.Title
        };
        await _repository.AddAsync(config, cancellationToken);
        return config.Id;
    }
}
