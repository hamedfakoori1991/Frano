using Framework.Domain.Entities;
using MediatR;
using VantageView.Application.Repositories;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.Application.CrawlerConfigurations.Queries;

public record GetCrawlerConfigurationByIdQuery(string Id) : IRequest<Result<CrawlerConfiguration>>;

public class GetCrawlerConfigurationByIdQueryHandler : IRequestHandler<GetCrawlerConfigurationByIdQuery, Result<CrawlerConfiguration>>
{

    private readonly ICrawlerConfigurationRepository _repository;

    public GetCrawlerConfigurationByIdQueryHandler(ICrawlerConfigurationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CrawlerConfiguration>> Handle(GetCrawlerConfigurationByIdQuery request, CancellationToken cancellationToken)
    {
        var items = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return items;
    }
}
