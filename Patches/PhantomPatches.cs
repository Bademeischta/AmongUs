using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class PhantomPatches
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        [HarmonyPostfix]
        public static void UpdatePhantomVisibility(PlayerControl __instance)
        {
            if (PhantomManager.Instance.IsPhantom(__instance.PlayerId))
            {
                __instance.nameText.color = new Color(0, 0, 0, 0);
                __instance.cosmetics.layer = LayerMask.NameToLayer("Ghost");
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void AddPhantomButton(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || RoleManager.Instance.GetRole(localPlayer.PlayerId)?.RoleType != RoleType.Phantom)
            {
                return;
            }

            // This is a placeholder for a real UI button.
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var isPhantom = !PhantomManager.Instance.IsPhantom(localPlayer.PlayerId);
                RpcManager.Instance.SendCmdEnterPhantomState(isPhantom);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void HidePhantomInMeeting(MeetingHud __instance)
        {
            foreach (var player in __instance.playerStates)
            {
                if (PhantomManager.Instance.IsPhantom(player.TargetPlayerId))
                {
                    player.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
        [HarmonyPrefix]
        public static bool PreventKillWhilePhantom(KillButtonManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer != null && PhantomManager.Instance.IsPhantom(localPlayer.PlayerId))
            {
                return false; // Prevent kill
            }
            return true; // Allow kill
        }

        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowInUse))]
        [HarmonyPrefix]
        public static bool PreventSabotageWhilePhantom(MapBehaviour __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer != null && PhantomManager.Instance.IsPhantom(localPlayer.PlayerId))
            {
                return false; // Prevent opening sabotage map
            }
            return true; // Allow opening sabotage map
        }
    }
}
