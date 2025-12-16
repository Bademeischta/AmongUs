using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using System;
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
                switch (reader.Tag)
                {
                    case NetworkManager.RoleAssignmentRpcId:
                        HandleRoleAssignment(reader);
                        break;
                    case NetworkManager.GameOptionsSyncRpcId:
                        HandleGameOptionsSync(reader);
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"[RPC Error] Failed to handle message with tag {reader.Tag}: {e}");
                reader.Position = initialPosition; // CRITICAL: Reset position to prevent desync
            }
        }

        private static void HandleRoleAssignment(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var roleType = (RoleType)reader.ReadByte();

            if (!Enum.IsDefined(typeof(RoleType), roleType))
            {
                Logger.LogWarning($"Invalid role type received in RPC: {(byte)roleType}");
                return;
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == playerId)
                {
                    RoleManager.Instance.SetRole(player, roleType);
                    return;
                }
            }
            Logger.LogWarning($"Player with ID {playerId} not found for role assignment.");
        }

        private static void HandleGameOptionsSync(MessageReader reader)
        {
            CustomGameOptions.JesterChance = reader.ReadSingle();
            Logger.LogInfo($"Received game options sync: JesterChance={CustomGameOptions.JesterChance}%");

            // Update the UI if the options menu is open
            GameOptionsMenuPatch.UpdateUI();
        }
    }
}
