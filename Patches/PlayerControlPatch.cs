using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(PlayerControl))]
    public static class PlayerControlPatch
    {
        [HarmonyPatch(nameof(PlayerControl.FixedUpdate))]
        [HarmonyPrefix]
        public static bool Prefix(PlayerControl __instance)
        {
            var puppeteer = RoleManager.Instance.GetRole(__instance.PlayerId) as PuppeteerRole;
            if (puppeteer != null && puppeteer.IsControlling && puppeteer.Target != null)
            {
                var writer = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte)CustomRpc.PuppeteerControl, Hazel.SendOption.Reliable);
                writer.Write(puppeteer.Target.PlayerId);
                writer.Write(__instance.transform.position);
                AmongUsClient.Instance.FinishRpc(writer);

                return false; // Prevent the puppeteer from moving
            }

            return true;
        }

        [HarmonyPatch(nameof(PlayerControl.Update))]
        [HarmonyPostfix]
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance != PlayerControl.LocalPlayer) return;

            var puppeteer = RoleManager.Instance.GetRole(__instance.PlayerId) as PuppeteerRole;
            if (puppeteer == null || !puppeteer.IsSelectingTarget) return;

            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    var target = hit.collider.GetComponent<PlayerControl>();
                    if (target != null && target != __instance)
                    {
                        puppeteer.SetTarget(target);
                        puppeteer.StartControl();
                        puppeteer.StopSelection();
                    }
                }
            }
        }
    }
}
