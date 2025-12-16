using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using System.Collections.Generic;

namespace MyCustomRolesMod.Patches
{
    public static class HandshakeManager
    {
        public const byte ProtocolVersion = 4;

        // Host-side: Keep track of clients that have not yet been verified.
        public static readonly HashSet<int> UnverifiedClients = new HashSet<int>();
    }

    [HarmonyPatch(typeof(GameLobby), nameof(GameLobby.OnPlayerJoined))]
    public static class HandshakePatch
    {
        public static void Postfix(GameLobby __instance, [HarmonyArgument(0)] PlayerControl newPlayer)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (newPlayer == PlayerControl.LocalPlayer) return;

            HandshakeManager.UnverifiedClients.Add(newPlayer.OwnerId);

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.VersionCheck);
            writer.Write(HandshakeManager.ProtocolVersion);
            writer.EndMessage();

            // This is a handshake message, it doesn't need the full ACK/retry treatment.
            AmongUsClient.Instance.SendOrDisconnect(writer, newPlayer.OwnerId);
        }
    }
}
