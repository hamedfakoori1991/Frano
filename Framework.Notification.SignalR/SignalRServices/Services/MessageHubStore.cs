using System.Collections.Concurrent;

public class MessageHubStore
{
    private static readonly ConcurrentDictionary<string, List<string>> UserConnectionIds = new();

    public bool TrySetUserConnectionId(string userId, string connectionId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(connectionId))
            return false;

        UserConnectionIds.AddOrUpdate(userId,
            new List<string> { connectionId },
            (_, existingConnectionIds) =>
            {
                existingConnectionIds.Add(connectionId);
                return existingConnectionIds;
            });

        return true;
    }

    public bool TryGetUserConnectionIds(string userId, out List<string> connectionIds)
    {
        return UserConnectionIds.TryGetValue(userId, out connectionIds);
    }

    public bool TryRemoveUserConnectionId(string userId, string connectionId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(connectionId))
            return false;

        if (UserConnectionIds.TryGetValue(userId, out List<string> connectionIds))
        {
            if (connectionIds.Remove(connectionId))
            {
                if (connectionIds.Count == 0)
                {
                    UserConnectionIds.TryRemove(userId, out _);
                }
                return true;
            }
        }

        return false;
    }
}
