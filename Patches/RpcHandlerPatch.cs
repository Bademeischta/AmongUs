using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Management;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HandleMessage))]
    public static class RpcHandlerPatch
    {
        private const byte CustomRpcId = 223;

        public static void Postfix(InnerNetClient __instance, MessageReader reader)
        {
            if (reader.Tag == CustomRpcId)
            {
                var playerId = reader.ReadByte();
                var roleType = (RoleType)reader.ReadByte();

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == playerId)
                    {
                        RoleManager.SetRole(player, roleType);
                        break;
                    }
                }
            }
        }
    }
}
