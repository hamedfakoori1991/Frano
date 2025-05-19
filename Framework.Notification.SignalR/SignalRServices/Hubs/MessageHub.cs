using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

[Authorize]
public class MessageHub : Hub<IMessageHub>
{
    private readonly MessageHubStore _hubStore;

    public MessageHub(MessageHubStore hubStore)
    {
        _hubStore = hubStore;
    }

    public static string HubRoute => "/messaging";
    public async Task JoinByGroups(params string[] groups)
    {
        foreach (var grp in groups.Where(w => !string.IsNullOrEmpty(w)))
            await Groups.AddToGroupAsync(Context.ConnectionId, grp);
    }

    public override async Task OnConnectedAsync()
    {
        string username = CurrentUser;

        if (string.IsNullOrEmpty(username))
            await base.OnConnectedAsync();

        if (string.IsNullOrEmpty(Context?.ConnectionId))
            await base.OnConnectedAsync();

        JoinTheCaller(username);

        var httpContext = Context?.GetHttpContext();
        if (httpContext != null)
        {
            httpContext.Request.Query.TryGetValue("Template-Id", out var templateId);
            if (Guid.TryParse(templateId, out var templateGuid))
            {
                await JoinByGroups(templateGuid.ToString());
            }
        }

        await base.OnConnectedAsync();
    }

    private void JoinTheCaller(string username)
    {
        _hubStore.TrySetUserConnectionId(username, Context.ConnectionId);
    }

    private void RemoveTheCaller(string username)
    {
        _hubStore.TryRemoveUserConnectionId(username, Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string username = CurrentUser;

        if (string.IsNullOrEmpty(username))
            await base.OnDisconnectedAsync(exception);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, username);

        RemoveTheCaller(username);
        await base.OnDisconnectedAsync(exception);
    }

    public string GetConnectionId()
    {
        return Context.ConnectionId;
    }
    private string CurrentUser => Context?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";

}


