using HarmonyLib;
using MyCustomRolesMod.Core;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
    public static class RoundEndPatch
    {
        public static void Postfix()
        {
            WitnessManager.Instance.ClearAllTestimonies();
            PuppeteerManager.Instance.ClearAllForcedMessages();
            GlitchManager.Instance.ClearAllCorruptedSystems();
            GlitchPatch.ClearCorruptibleSystems();
        }
    }
}
