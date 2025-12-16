using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class RoleManager
    {
        private static RoleManager _instance;
        public static RoleManager Instance => _instance ??= new RoleManager();

        private readonly ConcurrentDictionary<byte, BaseRole> _playerRoles = new ConcurrentDictionary<byte, BaseRole>();

        private byte _jesterWinnerId = byte.MaxValue;
        private readonly object _winnerLock = new object();

        public bool HasJesterWinner
        {
            get
            {
                lock (_winnerLock) { return _jesterWinnerId != byte.MaxValue; }
            }
        }

        private RoleManager() { }

        public void SetRole(PlayerControl player, RoleType roleType)
        {
            if (player == null) return;

            _playerRoles.TryRemove(player.PlayerId, out _);

            BaseRole newRole = roleType switch
            {
                RoleType.Jester => new JesterRole(player),
                _ => null
            };

            if (newRole != null)
            {
                _playerRoles[player.PlayerId] = newRole;
                ModPlugin.Logger.LogInfo($"[RoleManager] Assigned {roleType} to player {player.PlayerId}.");
            }
        }

        public BaseRole GetRole(byte playerId)
        {
            _playerRoles.TryGetValue(playerId, out var role);
            return role;
        }

        public Dictionary<byte, RoleType> GetAllRoles()
        {
            var roles = new Dictionary<byte, RoleType>();
            foreach (var pair in _playerRoles)
            {
                roles[pair.Key] = pair.Value.RoleType;
            }
            return roles;
        }

        public void ClearAllRoles()
        {
            _playerRoles.Clear();
            lock (_winnerLock)
            {
                _jesterWinnerId = byte.MaxValue;
            }
            ModPlugin.Logger.LogInfo("[RoleManager] All custom roles cleared.");
        }

        public void SetJesterWinner(byte playerId)
        {
            lock (_winnerLock)
            {
                if (_jesterWinnerId != byte.MaxValue)
                {
                    ModPlugin.Logger.LogWarning($"[RoleManager] Jester winner already set to {_jesterWinnerId}. Ignoring new winner {playerId}.");
                    return;
                }
                _jesterWinnerId = playerId;
                ModPlugin.Logger.LogInfo($"[RoleManager] Jester winner set: {playerId}");
            }
        }
    }
}
