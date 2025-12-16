using HarmonyLib;
using MyCustomRolesMod.Management;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class ExileControllerWrapUpPatch
    {
        public static bool Prefix(ExileController __instance)
        {
            var exiledPlayer = __instance.exiled;
            if (exiledPlayer == null || exiledPlayer.Object == null) return true;

            PlayerControl exiledPlayerControl = exiledPlayer.Object;
            if (RoleManager.GetRole(exiledPlayerControl) is JesterRole)
            {
                // Mark the jester as the winner
                RoleManager.JesterWinnerId = exiledPlayerControl.PlayerId;

                // Trigger a standard Impostor win. Our other patch will catch this.
                ShipStatus.Instance.RpcEndGame(EndGameReason.ImpostorByVote, false);

                // Prevent the original exile animation from playing
                __instance.gameObject.SetActive(false);
                return false;
            }

            return true; // Proceed with the original method
        }
    }

    [HarmonyPatch(typeof(EndGameScreen), nameof(EndGameScreen.SetVictoryText))]
    public static class EndGameScreenSetVictoryTextPatch
    {
        public static void Prefix(EndGameScreen __instance, ref string victoryText)
        {
            if (RoleManager.JesterWinnerId != byte.MaxValue)
            {
                // A Jester has won, so we change the victory text.
                victoryText = "Jester Wins";

                // Also, let's change the colors to pink
                __instance.WinText.color = Color.magenta;
                __instance.BackgroundBar.color = Color.magenta;
            }
        }
    }
}
