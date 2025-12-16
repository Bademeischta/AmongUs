using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class MeetingPatch
    {
        public static bool Prefix(ExileController __instance)
        {
            var exiledPlayer = __instance.exiled?.Object;
            if (exiledPlayer == null) return true;

            if (RoleManager.Instance.GetRole(exiledPlayer.PlayerId) is JesterRole)
            {
                RoleManager.Instance.SetJesterWinner(exiledPlayer.PlayerId);

                // HACK: We hijack ImpostorByVote because Among Us doesn't support custom win reasons.
                // This will show as an Impostor win in vanilla stats/achievements.
                // Our SetVictoryText patch corrects the display text.
                // WARNING: May conflict with other mods that also hijack this reason!
                ShipStatus.Instance.RpcEndGame(EndGameReason.ImpostorByVote, false);

                __instance.gameObject.SetActive(false);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(EndGameScreen), nameof(EndGameScreen.SetVictoryText))]
    public static class EndGameScreenPatch
    {
        public static void Prefix(EndGameScreen __instance, ref string victoryText)
        {
            if (RoleManager.Instance.HasJesterWinner)
            {
                victoryText = "Jester Wins";
                __instance.WinText.color = Color.magenta;
                __instance.BackgroundBar.color = Color.magenta;
            }
        }
    }
}
