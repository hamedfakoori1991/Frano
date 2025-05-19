using Framework.Domain.Entities;
using MapsterMapper;
using MediatR;
using VantageView.Application.Repositories;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.Application.CrawlerConfigurations.Commands;
public record UpdateCrawlerConfigurationCommand(string Id, string Title) : IRequest<Result>;

public class UpdateCrawlerConfigurationCommandHandler : IRequestHandler<UpdateCrawlerConfigurationCommand, Result>
{
    private readonly ICrawlerConfigurationRepository _repository;
    private readonly IMapper _mapper;

    public UpdateCrawlerConfigurationCommandHandler(ICrawlerConfigurationRepository repo, IMapper mapper)
    {
        _repository = repo;
        _mapper = mapper;
    }
    public async Task<Result> Handle(UpdateCrawlerConfigurationCommand request, CancellationToken cancellationToken)
    {
        var config = _mapper.Map<CrawlerConfiguration>(request);
        await _repository.UpdateAsync(config, cancellationToken);
        return Result.Ok();
    }
}
