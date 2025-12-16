using Hazel;
using MyCustomRolesMod.Roles;
using static MyCustomRolesMod.ModPlugin;

namespace MyCustomRolesMod.Management
{
    public static class NetworkManager
    {
        internal const byte RoleAssignmentRpcId = 223;
        internal const byte JesterWinRpcId = 224;
        internal const byte GameOptionsSyncRpcId = 225;

        public static void SendRoleAssignment(PlayerControl player, RoleType roleType)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            Logger.LogInfo($"Sending RPC: Assign {roleType} to Player {player.PlayerId}");
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage(RoleAssignmentRpcId);
            writer.Write(player.PlayerId);
            writer.Write((byte)roleType);
            writer.EndMessage();

            AmongUsClient.Instance.SendOrDisconnect(writer);
        }
    }
}
