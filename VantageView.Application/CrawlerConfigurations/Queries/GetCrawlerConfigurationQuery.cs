using Framework.Domain.Entities;
using MediatR;
using VantageView.Application.Repositories;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.Application.CrawlerConfigurations.Queries;

public record GetAllCrawlerConfigurationsQuery() : IRequest<Result<IReadOnlyList<CrawlerConfiguration>>>;

public class GetCrawlerConfigurationQueryHandler : IRequestHandler<GetAllCrawlerConfigurationsQuery, Result<IReadOnlyList<CrawlerConfiguration>>>
{

    private readonly ICrawlerConfigurationRepository _repository;

    public GetCrawlerConfigurationQueryHandler(ICrawlerConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<CrawlerConfiguration>>> Handle(GetAllCrawlerConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);
        return items;
    }
}
