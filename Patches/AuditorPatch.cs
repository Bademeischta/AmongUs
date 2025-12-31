using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class AuditorPatch
    {
        internal static KillButton _auditButton;
        internal static TMPro.TextMeshPro _auditText;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void HudManagerUpdatePatch(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || !localPlayer.CanMove)
            {
                if (_auditButton != null) _auditButton.gameObject.SetActive(false);
                return;
            }

            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role == null || role.RoleType != RoleType.Auditor)
            {
                if (_auditButton != null) _auditButton.gameObject.SetActive(false);
                return;
            }

            if (_auditButton == null)
            {
                var reportButton = __instance.ReportButton;
                _auditButton = Object.Instantiate(reportButton, reportButton.transform.parent);
                _auditButton.graphic.sprite = __instance.KillButton.graphic.sprite; // Use the kill button sprite for now
                _auditButton.name = "AuditButton";
                _auditButton.transform.localPosition = __instance.KillButton.transform.localPosition;
                _auditButton.gameObject.SetActive(true);
            }

            var auditor = (AuditorRole)role;
            var closestPlayer = FindClosestPlayer(localPlayer);
            _auditButton.gameObject.SetActive(true);
            _auditButton.SetTarget(closestPlayer);
            _auditButton.SetEnabled(closestPlayer != null);

            if (_auditButton.isCoolingDown) return;

            if (_auditButton.didPress)
            {
                _auditButton.didPress = false;
                if (closestPlayer != null)
                {
                    RpcManager.Instance.SendAuditPlayer(closestPlayer.PlayerId);
                    _auditButton.isCoolingDown = true;
                    _auditButton.cooldownTimer = PlayerControl.GameOptions.KillCooldown;
                }
            }
        }

        private static PlayerControl FindClosestPlayer(PlayerControl sourcePlayer)
        {
            PlayerControl closestPlayer = null;
            var minDistance = float.MaxValue;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == sourcePlayer || player.Data.IsDead) continue;

                var distance = Vector2.Distance(sourcePlayer.GetTruePosition(), player.GetTruePosition());
                if (distance < minDistance && distance < GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance])
                {
                    minDistance = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Populate))]
        [HarmonyPostfix]
        public static void MeetingHudPopulatePatch(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role == null || role.RoleType != RoleType.Auditor) return;

            var snapshot = AuditorManager.Instance.GetSnapshot();
            if (snapshot == null) return;

            var auditedPlayerId = AuditorManager.Instance.GetAuditedPlayerId();
            var auditedPlayer = GameData.Instance.GetPlayerById(auditedPlayerId)?.Object;
            if (auditedPlayer == null) return;

            if (_auditText == null)
            {
                _auditText = new GameObject("AuditText").AddComponent<TMPro.TextMeshPro>();
                _auditText.transform.SetParent(__instance.transform);
                _auditText.transform.localPosition = new Vector3(0, 2, 0);
                _auditText.fontSize = 1.5f;
                _auditText.alignment = TMPro.TextAlignmentOptions.Center;
            }

            _auditText.gameObject.SetActive(true);
            _auditText.text = $"Audit of {auditedPlayer.Data.PlayerName}:\n" +
                              $"Position: {snapshot.Position}\n" +
                              $"Tasks Completed: {snapshot.TasksCompleted}";
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
        [HarmonyPostfix]
        public static void MeetingHudClosePatch()
        {
            if (_auditText != null)
            {
                _auditText.gameObject.SetActive(false);
            }
        }
    }
}
