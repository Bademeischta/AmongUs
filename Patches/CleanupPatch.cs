using HarmonyLib;
using MyCustomRolesMod.Core;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class CleanupPatch
    {
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
        [HarmonyPostfix]
        public static void ShipStatusOnDestroyPatch()
        {
            AuditorManager.Instance.Clear();
            PhantomManager.Instance.Clear();

            AuditorPatch._auditButton = null;
            AuditorPatch._auditText = null;
            PhantomPatch._imprintButton = null;
        }
    }
}
