using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using System.Collections.Generic;

namespace MyCustomRolesMod.Patches
{
    public static class HandshakeManager
    {
        public const byte ProtocolVersion = 1;
        public static readonly List<int> UnverifiedClients = new List<int>();
        private static readonly object _lock = new object();

        public static void AddUnverifiedClient(int clientId)
        {
            lock (_lock)
            {
                if (!UnverifiedClients.Contains(clientId))
                {
                    UnverifiedClients.Add(clientId);
                }
            }
        }

        public static void RemoveUnverifiedClient(int clientId)
        {
            lock (_lock)
            {
                UnverifiedClients.Remove(clientId);
            }
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
    public static class JoinPatch
    {
        public static void Postfix(PlayerControl newPlayer)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            HandshakeManager.AddUnverifiedClient(newPlayer.OwnerId);

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.VersionCheck);
            writer.Write(HandshakeManager.ProtocolVersion);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer, newPlayer.OwnerId);
            writer.Recycle();
        }
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerLeft))]
    public static class LeavePatch
    {
        public static void Postfix(int clientId, bool amHost)
        {
            if (amHost)
            {
                HandshakeManager.RemoveUnverifiedClient(clientId);
            }
        }
    }
}
