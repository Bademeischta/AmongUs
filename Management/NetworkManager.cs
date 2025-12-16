using Hazel;
using MyCustomRolesMod.Management;

namespace MyCustomRolesMod.Management
{
    public static class NetworkManager
    {
        private const byte CustomRpcId = 223; // Using a higher ID to avoid conflicts

        public static void SendRoleAssignment(PlayerControl player, RoleType roleType)
        {
            if (!PlayerControl.LocalPlayer.AmOwner) return;

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage(CustomRpcId);
            writer.Write(player.PlayerId);
            writer.Write((byte)roleType);
            writer.EndMessage();

            AmongUsClient.Instance.SendOrDisconnect(writer);
            writer.Recycle();
        }
    }
}
