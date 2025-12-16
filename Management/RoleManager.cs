using System.Collections.Generic;
using MyCustomRolesMod.Roles;

namespace MyCustomRolesMod.Management
{
    public enum RoleType : byte
    {
        None,
        Jester
    }

    public static class RoleManager
    {
        private static readonly Dictionary<byte, BaseRole> PlayerRoles = new Dictionary<byte, BaseRole>();
        public static byte JesterWinnerId { get; set; } = byte.MaxValue;

        public static void SetRole(PlayerControl player, RoleType roleType)
        {
            if (player == null) return;

            // Remove existing role if any
            if (PlayerRoles.ContainsKey(player.PlayerId))
            {
                PlayerRoles.Remove(player.PlayerId);
            }

            // Assign new role
            BaseRole newRole = roleType switch
            {
                RoleType.Jester => new JesterRole(player),
                _ => null
            };

            if (newRole != null)
            {
                PlayerRoles.Add(player.PlayerId, newRole);
                newRole.OnRoleAssign();
            }
        }

        public static BaseRole GetRole(PlayerControl player)
        {
            if (player == null) return null;
            PlayerRoles.TryGetValue(player.PlayerId, out var role);
            return role;
        }

        public static void ClearRoles()
        {
            foreach (var role in PlayerRoles.Values)
            {
                role.OnRoleClear();
            }
            PlayerRoles.Clear();
            JesterWinnerId = byte.MaxValue;
        }
    }
}
