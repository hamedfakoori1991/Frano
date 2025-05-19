namespace Framework.Domain.Entities.Enums;

public enum ErrorType
{
    Validation,
    NotFound,
    Authentication,
    Authorization,
    Database,
    Translation,
    Unexpected,
    ExecutionPlan
}
