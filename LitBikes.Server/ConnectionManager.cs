using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LitBikes.Server
{
    public class ConnectionManager
    {
        private readonly Dictionary<string, Guid> _playerConnections;

        public ConnectionManager()
        {
            _playerConnections = new Dictionary<string, Guid>();
        }

        public void OnConnected(string connectionId)
        {
            if (!PlayerExists(connectionId, out _))
            {
                var newPlayerId = Guid.NewGuid();
                _playerConnections.Add(connectionId, newPlayerId);
            }
        }

        public bool PlayerExists(string connectionId, out Guid playerId)
        {
            var exists = _playerConnections.TryGetValue(connectionId, out var fetchedPlayerId);
            if (exists)
                playerId = fetchedPlayerId;
            return exists;
        }
    }
}
