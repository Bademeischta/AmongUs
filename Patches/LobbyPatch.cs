using HarmonyLib;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(GameLobby), nameof(GameLobby.OnPlayerJoined))]
    public static class LobbyPatch
    {
        public static void Postfix(GameLobby __instance, [HarmonyArgument(0)] PlayerControl newPlayer)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (newPlayer == PlayerControl.LocalPlayer) return; // Don't send to self

            ModPlugin.Logger.LogInfo($"[Lobby] Player {newPlayer.PlayerId} joined. Syncing options to them.");

            var packet = new OptionsPacket
            {
                JesterChance = ModPlugin.ModConfig.JesterChance.Value
            };

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SyncOptions);
            packet.Serialize(writer);
            writer.EndMessage();

            RpcManager.Instance.SendTo(writer, newPlayer.OwnerId);
        }
    }
}
