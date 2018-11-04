using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LitBikes.Server
{
    public static class ConnectionManager
    {
        private static readonly ConcurrentDictionary<string, Guid> ConnectionsToPlayers = new ConcurrentDictionary<string, Guid>();
        private static readonly ConcurrentDictionary<Guid, string> PlayersToConnections = new ConcurrentDictionary<Guid, string>();

        public static void OnConnected(string connectionId)
        {
            if (PlayerExists(connectionId, out _)) return;
            var newPlayerId = Guid.NewGuid();
            ConnectionsToPlayers.TryAdd(connectionId, newPlayerId);
            PlayersToConnections.TryAdd(newPlayerId, connectionId);
        }

        public static void OnDisconnected(string connectionId)
        {
            ConnectionsToPlayers.TryRemove(connectionId, out var playerId);
            PlayersToConnections.TryRemove(playerId, out _);
        }

        public static bool PlayerExists(string connectionId, out Guid playerId)
        {
            var exists = ConnectionsToPlayers.TryGetValue(connectionId, out var fetchedPlayerId);
            if (exists)
                playerId = fetchedPlayerId;
            return exists;
        }

        public static bool GetConnectionId(Guid playerId, out string connectionId)
        {
            var exists = PlayersToConnections.TryGetValue(playerId, out var fetchedConnectionId);
            connectionId = exists ? fetchedConnectionId : null;
            return exists;
        }
    }
}
