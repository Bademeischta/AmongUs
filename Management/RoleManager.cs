using System;
using System.Collections.Generic;
using MyCustomRolesMod.Roles;

namespace MyCustomRolesMod.Management
{
    public class RoleManager
    {
        private static RoleManager _instance;
        public static RoleManager Instance => _instance ??= new RoleManager();

        private readonly Dictionary<byte, BaseRole> _playerRoles = new Dictionary<byte, BaseRole>();

        public event Action<PlayerControl, RoleType> OnRoleAssigned;

        public byte JesterWinnerId { get; set; } = byte.MaxValue;

        private RoleManager() { }

        public void SetRole(PlayerControl player, RoleType roleType)
        {
            if (player == null) return;

            ClearRole(player.PlayerId);

            BaseRole newRole = roleType switch
            {
                RoleType.Jester => new JesterRole(player),
                RoleType.Echo => new EchoRole(player),
                RoleType.Puppeteer => new PuppeteerRole(player),
                _ => null
            };

            if (newRole != null)
            {
                _playerRoles[player.PlayerId] = newRole;
                newRole.OnRoleAssign();
                OnRoleAssigned?.Invoke(player, roleType);
                ModPlugin.Logger.LogInfo($"Assigned {roleType} to player {player.PlayerId}.");
            }
        }

        public BaseRole GetRole(byte playerId)
        {
            _playerRoles.TryGetValue(playerId, out var role);
            return role;
        }

        public IEnumerable<BaseRole> GetAllRoles()
        {
            return _playerRoles.Values;
        }

        public void ClearRole(byte playerId)
        {
            if (_playerRoles.TryGetValue(playerId, out var oldRole))
            {
                oldRole.OnRoleClear();
                _playerRoles.Remove(playerId);
            }
        }

        public void ClearAllRoles()
        {
            foreach (var role in _playerRoles.Values)
            {
                role.OnRoleClear();
            }
            _playerRoles.Clear();
            JesterWinnerId = byte.MaxValue;
            ModPlugin.Logger.LogInfo("All custom roles cleared.");
        }
    }
}
