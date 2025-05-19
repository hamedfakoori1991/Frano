using Framework.WebApi;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VantageView.Application.CrawlerConfigurations.Commands;
using VantageView.Application.CrawlerConfigurations.Queries;
using VantageView.Contracts.CrawlerConfigurations;
using VantageView.Domain.CrawlerConfigurations;

namespace VantageView.RestApi.Controllers;

public class CrawlerConfigurationsController : AppController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CrawlerConfigurationsController(IMediator mediator,
        IMapper mapper, ILogger<CrawlerConfigurationsController> logger) : base(mapper, logger)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] SetupNewCrawlerConfigurationRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<SetupANewCrawlerConfigurationCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);
        return PrepareResponse(result);
    }


    [ProducesResponseType(typeof(List<CrawlerConfigurationResponse>), 200)]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCrawlerConfigurationsQuery(), cancellationToken);
        return PrepareResponse<List<CrawlerConfiguration>, List<CrawlerConfigurationResponse>>(result.Data.ToList());
    }

    [ProducesResponseType(typeof(CrawlerConfigurationResponse), 200)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _mediator.Send(new GetCrawlerConfigurationByIdQuery(id));
        return PrepareResponse<CrawlerConfiguration, CrawlerConfigurationResponse>(result);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateCrawlerConfigurationRequest request)
    {
        var command = _mapper.Map<UpdateCrawlerConfigurationCommand>(request);
        var result = await _mediator.Send(command);
        return PrepareResponse(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteCrawlerConfigurationCommand(id));
        return PrepareResponse(result);
    }
}

