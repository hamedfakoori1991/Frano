

using Framework.Domain.Entities;
using Framework.WebApi.Contracts;
using Framework.WebApi.Extensions;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Framework.WebApi;

[ApiController]
[Route("api/[controller]")]
public class AppController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AppController(IMapper mapper, ILogger logger)
    {
        _mapper = mapper;
        _logger = logger;
    }
    protected IActionResult PrepareResponse<T>(Result<T> result)
    {
        if (result.HasError)
            return result.GetMessagesResult(_logger);

        ApiResult<T> apiResult = result;

        return Ok(apiResult);
    }
    protected IActionResult PrepareResponse<TSource, TDestination>(Result<TSource> result)
    {
        if (result.HasError)
            return result.GetMessagesResult(_logger);

        var res = Result<TDestination>.Ok(_mapper.Map<TDestination>(result.Data!), result.Messages.ToArray());
        ApiResult<TDestination> apiResult = res;
        return Ok(apiResult);
    }
    protected IActionResult PrepareFileResponse<TSource>(Result<TSource> result)
    {
        if (result.HasError)
            return result.GetMessagesResult(_logger);

        var res = _mapper.Map<FileStreamResult>(result.Data!);
       
        return res;
    }
    protected IActionResult PrepareResponse(Result result)
    {
        return result.GetMessagesResult(_logger);
    }
}

