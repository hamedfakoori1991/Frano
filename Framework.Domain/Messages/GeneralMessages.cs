using Microsoft.Extensions.Logging;

namespace Framework.Domain.Messages;

public static class GeneralMessages
{
    public static readonly Message NotFoundByIds = Message.CreateError("General.NotFoundByIds", "No Result For {0}-Ids:{1} of the resource:{2}", "", ErrorType.NotFound);
    public static Message NotFoundByNames() => Message.CreateError("General.NotFoundByNames", "No Result For Names:{0} of the resource:{1}", "", ErrorType.NotFound);
    public static Message NotFoundById() => Message.CreateError("General.NotFoundById", "No Result For Specified Id:{0} of the resource:{1}", "", ErrorType.NotFound);
    public static readonly Message NotFoundByType = Message.CreateError("General.NotFoundByType", "No Result For Specified Type:{0}", "", ErrorType.NotFound);
    public static readonly Message DataIsNull = Message.CreateError("General.DataIsNull", "No Result found", "", ErrorType.NotFound);
    public static readonly Message DuplicateFoundById = Message.CreateError("General.DuplicateFoundById", "More Than One Result Found For The Specified Id {0} of the resource :{1}", "", ErrorType.Validation);
    public static readonly Message SaveChangeProblem = Message.CreateError("General.SaveChangeProblem", "Save Change gets failed", "", ErrorType.Database);
    public static readonly Message InvalidRequestedData = Message.CreateError("General.Validation", "Request Is Invalid", "", ErrorType.Validation);
    public static readonly Message FailedToProcess = Message.CreateError("General.Validation", "Request Failed", "", ErrorType.Validation);
    public static readonly Message UnexpectedError = Message.CreateError("General.UnexpectedError", "Unexpected Error message:{0}, inner exception:{1}", "", ErrorType.Unexpected);
    public static readonly Message DbError = Message.CreateError("General.DbError", "Error message:{0}", "", ErrorType.Database);
    public static readonly Message DbErrorWithInnerEx = Message.CreateError("General.DbError", "Error message:{0}, inner exception:{1}", "", ErrorType.Database);
    public static readonly Message ValidationError = Message.CreateError("General.Validation", "{0}", "", ErrorType.Validation);
    public static readonly Message ConditionCanNotBeNull = Message.CreateError("General.Validation", "Condition Can Not Be Null", "", ErrorType.Validation);
    public static readonly Message FailedToUpdateDataBase = Message.CreateError("General.FailedToUpdateDataBase", "Unexpected Failure while updating Database", "", ErrorType.Database);
    public static readonly Message MoreThanOneItemFound = Message.CreateError("General.Validation", "More Than One Item Got Found", "", ErrorType.Validation);
    public static readonly Message PermissionDenied = Message.CreateError("General.PermissionDenied", "You do not have the necessary permissions to perform this action.", "", ErrorType.Validation);
    public static Message ListMustHaveOneItem() => Message.CreateError("General.ListMustHasOneItem", "{0} must has at least one item.", "", ErrorType.Validation);
    public static Message TwoListMustHaveEqualItems() => Message.CreateError("General.TwoListMustHaveEqualItems", "{0} and {1} must have equal items.", "", ErrorType.Validation);
    public static Message RequiredField() => Message.CreateError("General.RequiredField", "The '{0}' is(are) required", "", ErrorType.Validation);
    public static Message InvalidNameChars() => Message.CreateError("General.InvalidNameChars", "'{0}' with value '{1}' must start with a letter, contain only alphanumeric characters, underscores(_) and character length 150.", "", ErrorType.Validation);
    public static Message InvalidLabelChars() => Message.CreateError("General.InvalidLabelChars", "'{0}' with value '{1}' must start with a letter, contain only alphanumeric characters, spaces, underscores(_), character length 150 and no leading spaces.", "", ErrorType.Validation);
}

public static partial class Logs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Handling request {RequestTypeName} has been started.", SkipEnabledCheck = false)]
    public static partial void General_RequestHandlingStarted(this ILogger logger, string requestTypeName);

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "The request {RequestTypeName} with payload {@RequestData} gets failed with messages : {@Error}", SkipEnabledCheck = false)]
    public static partial void General_RequestGetsFailed(this ILogger logger, string requestTypeName, object RequestData, object error);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Handling request {RequestTypeName} has been successfully finished.", SkipEnabledCheck = false)]
    public static partial void General_RequestHandledSuccessfully(this ILogger logger, string requestTypeName);

    [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "There is no fluent-validator for the: {RequestTypeName}.", SkipEnabledCheck = false)]
    public static partial void General_ThereIsNoValidatorForRequestedType(this ILogger logger, string requestTypeName);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "The Request has been failed by: {Message}.", SkipEnabledCheck = false)]
    public static partial void General_FailedMessage(this ILogger logger, MessageResponse message);

    [LoggerMessage(EventId = 6, Level = LogLevel.Information, Message = "Http request: {Path} has validated successfullty for {RequestTypeName}.", SkipEnabledCheck = false)]
    public static partial void General_ApiRequestValidatedSuccessfully(this ILogger logger, string path, string requestTypeName);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "The request {RequestTypeName} with payload {@RequestData} gets failed with messages : {@Error}", SkipEnabledCheck = false)]
    public static partial void General_ApiRequestGetsFailed(this ILogger logger, string requestTypeName, object RequestData, object error);


}
