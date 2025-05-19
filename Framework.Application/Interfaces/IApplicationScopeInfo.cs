using System.Security.Claims;

namespace Framework.Application.Interfaces;

public interface IApplicationScopeInfo
{
    string Mte { get; }
    void SetTenant(string tenant);
    void SetMte(string mte);
    void SetUserId(string userId);
    void SetUserName(string userName);
    void SetClaimsPrincipal(ClaimsPrincipal? user);
    void SetDefaultLanguage(string language);
    string GetLanguage();
    string Tenant { get; }
    ClaimsPrincipal ClaimsPrincipal { get; }
    string? GetCurrentUserId();
    string? GetCurrentUserName();

}
