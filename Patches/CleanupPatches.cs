using HarmonyLib;
using MyCustomRolesMod.Core;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class CleanupPatches
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
        [HarmonyPostfix]
        public static void Cleanup()
        {
            RoleManager.Instance.ClearAllRoles();
            ScribeManager.Instance.Clear();
            PhantomManager.Instance.Clear();
            ScribePatches._scribePlayerId = byte.MaxValue;
        }
    }
}
