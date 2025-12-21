using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using System.Collections.Generic;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class ScribePatches
    {
        internal static byte _scribePlayerId = byte.MaxValue;

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        [HarmonyPostfix]
        public static void AssignHiddenTruth()
        {
            if (!AmongUsClient.Instance.AmHost) return;

            var scribe = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => RoleManager.Instance.GetRole(p.PlayerId)?.RoleType == RoleType.Scribe);
            if (scribe == null) return;

            _scribePlayerId = scribe.PlayerId;

            var truths = new List<string>
            {
                $"A kill occurred in {GetRandomLocation()}.",
                $"{GetRandomPlayerName()} is not an Impostor.",
                $"{GetRandomPlayerName()} visited {GetRandomLocation()}.",
                $"The Impostors are {GetImpostorNames()}."
            };

            var truth = truths[UnityEngine.Random.Range(0, truths.Count)];
            ScribeManager.Instance.SetHiddenTruth(truth);
            RpcManager.Instance.SendSyncHiddenTruth(_scribePlayerId, truth);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        [HarmonyPrefix]
        public static void RevealTruthOnDeath(PlayerControl __instance, PlayerControl CACHEDTARGET)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            if (CACHEDTARGET.PlayerId == _scribePlayerId)
            {
                var truth = ScribeManager.Instance.GetHiddenTruth();
                if (!string.IsNullOrEmpty(truth))
                {
                    RpcManager.Instance.SendRevealTruth(truth);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]
        public static void DisplayTruthInMeeting(MeetingHud __instance)
        {
            if (ScribeManager.Instance.IsTruthRevealed(out var truth))
            {
                // This is a placeholder for the actual UI implementation.
                // In a real mod, you would create a custom UI element.
                __instance.discussionTimerText.text += $"\n<color=#FFD700>Scribe's Truth: {truth}</color>";
            }
        }

        private static string GetRandomLocation()
        {
            var locations = new[] { "MedBay", "Electrical", "Security", "Reactor", "Admin" };
            return locations[UnityEngine.Random.Range(0, locations.Length)];
        }

        private static string GetRandomPlayerName()
        {
            return PlayerControl.AllPlayerControls[UnityEngine.Random.Range(0, PlayerControl.AllPlayerControls.Count)].name;
        }

        private static string GetImpostorNames()
        {
            return string.Join(", ", PlayerControl.AllPlayerControls.ToArray().Where(p => p.Data.IsImpostor).Select(p => p.name));
        }
    }
}
