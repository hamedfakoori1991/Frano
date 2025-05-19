using System.Security.Claims;
using Framework.Application.Interfaces;

namespace Framework.Application.Services;

public class ApplicationScopeInfo : IApplicationScopeInfo
{
    private string _tenant;
    private readonly string _customTenant;
    private string _language;
    private string _usaerId;
    private string _userName;

    public string Mte { get; private set; }
    public string Tenant => string.IsNullOrEmpty(_customTenant) ? _tenant : _customTenant;
    public ClaimsPrincipal? ClaimsPrincipal { get; private set; }

    public void SetTenant(string tenant) => _tenant = tenant;
    public void SetClaimsPrincipal(ClaimsPrincipal? user)
    {
        ClaimsPrincipal = user;
        SetUserId(ClaimsPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "");
        SetUserName(ClaimsPrincipal?.Claims?.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? "");
    }
    public void SetDefaultLanguage(string language) => _language = language;
    public string GetLanguage() => _language;

    public void SetUserId(string userId)
    {
        _usaerId = userId;
    }

    public void SetUserName(string userName)
    {
        _userName = userName;
    }

    public string? GetCurrentUserId()
    {
        return _usaerId;
    }

    public string? GetCurrentUserName()
    {
        return _userName;
    }

    public void SetMte(string mte)
    {
        Mte = mte;
    }
}
