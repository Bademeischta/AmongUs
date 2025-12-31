using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class PhantomPatch
    {
        internal static KillButton _imprintButton;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void HudManagerUpdatePatch(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || !localPlayer.CanMove)
            {
                if (_imprintButton != null) _imprintButton.gameObject.SetActive(false);
                return;
            }

            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role == null || role.RoleType != RoleType.Phantom)
            {
                if (_imprintButton != null) _imprintButton.gameObject.SetActive(false);
                return;
            }

            if (_imprintButton == null)
            {
                var killButton = __instance.KillButton;
                _imprintButton = Object.Instantiate(killButton, killButton.transform.parent);
                _imprintButton.graphic.sprite = killButton.graphic.sprite; // TODO: Replace with custom sprite
                _imprintButton.name = "ImprintButton";
                _imprintButton.transform.localPosition = killButton.transform.localPosition + new Vector3(1.5f, 0, 0);
            }

            _imprintButton.gameObject.SetActive(true);
            _imprintButton.SetTarget(null);
            _imprintButton.SetEnabled(true);

            if (_imprintButton.isCoolingDown) return;

            if (_imprintButton.didPress)
            {
                _imprintButton.didPress = false;
                RpcManager.Instance.SendSetImprint(localPlayer.GetTruePosition());
                _imprintButton.isCoolingDown = true;
                _imprintButton.cooldownTimer = PlayerControl.GameOptions.KillCooldown;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        [HarmonyPrefix]
        public static bool MurderPlayerPatch(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var role = RoleManager.Instance.GetRole(__instance.PlayerId);
            if (role == null || role.RoleType != RoleType.Phantom)
            {
                return true;
            }

            var imprintLocation = PhantomManager.Instance.GetImprintLocation();
            if (imprintLocation.HasValue)
            {
                RpcManager.Instance.SendPhantomKill(target.PlayerId);
                return false;
            }

            return true;
        }
    }
}
