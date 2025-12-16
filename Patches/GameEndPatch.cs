using HarmonyLib;
using MyCustomRolesMod.Management;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public static class GameEndPatch
    {
        public static void Postfix()
        {
            RoleManager.ClearRoles();
        }
    }
}
