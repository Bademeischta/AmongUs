using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnSpawn))]
    public static class JoinPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            if (GameData.Instance != null && GameData.Instance.GameStarted)
            {
                ModPlugin.Logger.LogInfo($"[Late Join] Player {__instance.PlayerId} joined mid-game. Syncing roles to them.");

                var allRoles = RoleManager.Instance.GetAllRoles();
                if (allRoles.Count > 0)
                {
                    var writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SyncAllRoles);
                    writer.Write((ushort)allRoles.Count);
                    foreach (var role in allRoles)
                    {
                        writer.Write(role.Key);
                        writer.Write((byte)role.Value);
                    }
                    writer.EndMessage();

                    RpcManager.Instance.SendTo(writer, __instance.OwnerId);
                }
            }
        }
    }
}
