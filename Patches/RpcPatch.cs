using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using System;
using System.Linq;
using static MyCustomRolesMod.ModPlugin;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HandleMessage))]
    public static class RpcPatch
    {
        public static void Postfix(InnerNetClient __instance, MessageReader reader)
        {
            var initialPosition = reader.Position;
            try
            {
                var rpcType = (RpcType)reader.Tag;

                if (rpcType == RpcType.Acknowledge)
                {
                    RpcManager.Instance.HandleAck(reader.ReadUInt32());
                    return;
                }

                var messageId = reader.ReadUInt32();
                var payload = reader.ReadBytesAndSize();
                var payloadReader = MessageReader.Get(payload);

                switch (rpcType)
                {
                    case RpcType.SetRole:
                        HandleSetRole(payloadReader);
                        break;
                    case RpcType.SyncAllRoles:
                        HandleSyncAllRoles(payloadReader);
                        break;
                    case RpcType.SyncOptions:
                        HandleSyncOptions(payloadReader);
                        break;
                    default:
                        Logger.LogWarning($"[RPC] Received unhandled message of type {rpcType}.");
                        break;
                }

                RpcManager.SendAck(messageId);
            }
            catch (Exception e)
            {
                Logger.LogError($"[RPC Error] Failed to handle message: {e}");
                reader.Position = initialPosition;
            }
        }

        private static void HandleSetRole(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var roleType = (RoleType)reader.ReadByte();
            var player = GameData.Instance.GetPlayerById(playerId)?.Object;
            if (player != null)
            {
                RoleManager.Instance.SetRole(player, roleType);
            }
        }

        private static void HandleSyncAllRoles(MessageReader reader)
        {
            var count = reader.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                var playerId = reader.ReadByte();
                var roleType = (RoleType)reader.ReadByte();
                var player = GameData.Instance.GetPlayerById(playerId)?.Object;
                if (player != null)
                {
                    RoleManager.Instance.SetRole(player, roleType);
                }
            }
        }

        private static void HandleSyncOptions(MessageReader reader)
        {
            var packet = OptionsPacket.Deserialize(reader);
            ModPlugin.ModConfig.JesterChance.Value = packet.JesterChance;
            Logger.LogInfo($"[RPC] Synced game options: JesterChance={packet.JesterChance}%");

            GameOptionsMenuPatch.UpdateUI();
        }
    }
}
