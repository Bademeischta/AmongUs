using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class ExileControllerWrapUpPatch
    {
        public static bool Prefix(ExileController __instance)
        {
            var exiledPlayer = __instance.exiled?.Object;
            if (exiledPlayer == null) return true;

            if (RoleManager.Instance.GetRole(exiledPlayer.PlayerId) is JesterRole)
            {
                ModPlugin.Logger.LogInfo($"Jester {exiledPlayer.PlayerId} was exiled. Triggering Jester win.");
                RoleManager.Instance.JesterWinnerId = exiledPlayer.PlayerId;

                // Hijack the impostor win flow. The EndGameScreen patch will modify the text.
                ShipStatus.Instance.RpcEndGame(EndGameReason.ImpostorByVote, false);

                // Prevent original exile animation
                __instance.gameObject.SetActive(false);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(EndGameScreen), nameof(EndGameScreen.SetVictoryText))]
    public static class EndGameScreenSetVictoryTextPatch
    {
        public static void Prefix(EndGameScreen __instance, ref string victoryText)
        {
            if (RoleManager.Instance.JesterWinnerId != byte.MaxValue)
            {
                victoryText = "Jester Wins";
                __instance.WinText.color = Color.magenta;
                __instance.BackgroundBar.color = Color.magenta;
            }
        }
    }
}
