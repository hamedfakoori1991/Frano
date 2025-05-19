using System.Net;
using Framework.Domain.Entities;
using Framework.Domain.Entities.Enums;
using Framework.Domain.Messages;
using Framework.WebApi.Contracts;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Framework.WebApi.Extensions;

public static class MessageResultExtensions
{
    public static IWebHostEnvironment? Environment { get; set; }

    public static ObjectResult GetMessagesResult(this Result result, ILogger logger)
    {
        HttpStatusCode status = result.HasError ? GetHttpStatusCode(result.Messages, result.ErrorType) : HttpStatusCode.OK;
        return GetMessageObject(result.Messages, status, logger);
    }

    public static ObjectResult GetMessagesResult(this Exception ex, ILogger logger)
    {
        List<Message> messages = new() { GeneralMessages.UnexpectedError.WithParams(ex.Message, ex.InnerException?.Message ?? string.Empty) };
        return GetMessageObject(messages, HttpStatusCode.BadRequest, logger);
    }

    private static ObjectResult GetMessageObject(List<Message> errors, HttpStatusCode status, ILogger logger)
    {
        ApiMessageResponse response = new();

        response.Messages.AddRange(errors.Select(z => z.GetMessage(logger)).ToList());
        response.StatusCode = (int)status;

        ObjectResult result = new(response)
        {
            StatusCode = (int)status
        };

        return result;
    }

    private static MessageResponse GetMessage(this Message message, ILogger logger)
    {
        var msg = message.Adapt<MessageResponse>();

        return msg;

        //TODO: As we are in the development phase it might be better to disable this part temporarily!
        //if (Environment?.IsDevelopment() == true)
        //    return msg;

        //return message.ErrorType is ErrorType.Database or ErrorType.Unexpected
        //    ? GeneralMessages.FailedToProcess.Adapt<MessageResponse>()
        //    : msg;
    }

    private static HttpStatusCode GetHttpStatusCode(List<Message> messages, ErrorType? errorType)
    {
        ErrorType? error = errorType ?? messages.FirstOrDefault()?.ErrorType;

        HttpStatusCode statusCode = error switch
        {
            ErrorType.Validation => HttpStatusCode.UnprocessableEntity,
            ErrorType.Authorization => HttpStatusCode.Forbidden,
            ErrorType.Authentication => HttpStatusCode.Unauthorized,
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Database => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.BadRequest
        };

        return statusCode;
    }
}
