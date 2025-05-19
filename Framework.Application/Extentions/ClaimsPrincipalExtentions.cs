using System.Security.Claims;
using Framework.Domain.Common;

namespace Framework.Application.Jobs.ExecutionPlan;

public static class ClaimsPrincipalExtentions
{
    public static string GetUserIdentifier(this ClaimsPrincipal user) => user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    public static string GetUserName(this ClaimsPrincipal user) => user?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "";
    public static string GetUserLanguage(this ClaimsPrincipal user) => user?.Claims?.FirstOrDefault(c => c.Type == DomainConsts.LanguageColumnName.ToLower())?.Value ?? "EN";

}



