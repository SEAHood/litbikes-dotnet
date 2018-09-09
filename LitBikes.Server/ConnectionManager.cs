using System;
using System.Collections.Generic;

namespace LitBikes.Server
{
    public static class ConnectionManager
    {
        private static readonly Dictionary<string, Guid> ConnectionsToPlayers = new Dictionary<string, Guid>();
        private static readonly Dictionary<Guid, string> PlayersToConnections = new Dictionary<Guid, string>();

        public static void OnConnected(string connectionId)
        {
            if (PlayerExists(connectionId, out _)) return;
            var newPlayerId = Guid.NewGuid();
            ConnectionsToPlayers.Add(connectionId, newPlayerId);
            PlayersToConnections.Add(newPlayerId, connectionId);
        }

        public static void OnDisconnected(string connectionId)
        {
            ConnectionsToPlayers.TryGetValue(connectionId, out var playerId);
            ConnectionsToPlayers.Remove(connectionId);
            PlayersToConnections.Remove(playerId);
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
