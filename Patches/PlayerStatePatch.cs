using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class PlayerStatePatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        [HarmonyPostfix]
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var puppeteer = RoleManager.Instance.GetRole(__instance.PlayerId) as PuppeteerRole;
            if (puppeteer != null && (target == __instance || target == puppeteer.Target))
            {
                puppeteer.StopControl();
                puppeteer.ClearTarget();
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in RoleManager.Instance.GetAllRoles())
            {
                if (role is PuppeteerRole puppeteer)
                {
                    puppeteer.StopControl();
                    puppeteer.ClearTarget();
                }
            }
        }
    }
}
