namespace Framework.Application.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class TransactionalAttribute : Attribute
{
}
