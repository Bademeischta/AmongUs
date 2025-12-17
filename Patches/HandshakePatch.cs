using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Networking;
using System.Collections.Generic;

namespace MyCustomRolesMod.Patches
{
    // A placeholder for handshake logic.
    public static class HandshakeManager
    {
        public const byte ProtocolVersion = 4;

        // Using a HashSet for efficient lookups.
        public static readonly HashSet<int> UnverifiedClients = new HashSet<int>();

        // A lock for thread-safe access to the client list.
        public static readonly object _lock = new object();
    }

    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.AddPlayer))]
    public static class GameStartManagerAddPlayerPatch
    {
        public static void Postfix(GameStartManager __instance, [HarmonyArgument(0)] PlayerControl newPlayer)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (newPlayer.OwnerId == AmongUsClient.Instance.ClientId) return; // Don't handshake with self.

            lock(HandshakeManager._lock)
            {
                HandshakeManager.UnverifiedClients.Add(newPlayer.OwnerId);
            }

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.VersionCheck);
            writer.Write(HandshakeManager.ProtocolVersion);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer, newPlayer.OwnerId);
        }
    }
}
