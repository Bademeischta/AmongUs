using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;
using TMPro;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class AuditorPatch
    {
        private static GameObject _auditButton;
        private static List<Console> _allConsoles = new List<Console>();

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static void Postfix(ShipStatus __instance)
        {
            _allConsoles = new List<Console>(Object.FindObjectsOfType<Console>());
        }

        [HarmonyPatch(typeof(PlayerTask), nameof(PlayerTask.OnComplete))]
        public static void Postfix(PlayerTask __instance)
        {
            var player = __instance.Owner;
            if (player != null)
            {
                var console = __instance.Minigame.MinigameHost.Console;
                if (console != null)
                {
                    AuditorManager.Instance.AddCompletedTask(player, console.transform.position);
                }
            }
        }

        [HarmonyPatch(typeof(global::HudManager), nameof(global::HudManager.Update))]
        public static void Postfix(global::HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || RoleManager.Instance.GetRole(localPlayer.PlayerId)?.RoleType != RoleType.Auditor || localPlayer.Data.IsDead)
            {
                _auditButton?.SetActive(false);
                return;
            }

            if (_auditButton == null && __instance.UseButton != null)
            {
                _auditButton = Object.Instantiate(__instance.UseButton.gameObject, __instance.UseButton.transform.parent);
                _auditButton.transform.localPosition = new Vector3(2.5f, 0.5f, 0);
                _auditButton.name = "AuditButton";

                var passiveButton = _auditButton.GetComponent<PassiveButton>();
                passiveButton.OnClick.RemoveAllListeners();
                passiveButton.OnClick.AddListener(OnAuditButtonClick);

                var text = _auditButton.GetComponentInChildren<TextMeshPro>();
                text.text = "Audit";
            }

            if (_auditButton != null)
            {
                Console auditableConsole = GetClosestAuditableConsole(localPlayer.GetTruePosition());
                bool canAudit = auditableConsole != null;

                var renderer = _auditButton.GetComponent<SpriteRenderer>();
                renderer.color = canAudit ? Color.white : Color.gray;
                var button = _auditButton.GetComponent<PassiveButton>();
                button.enabled = canAudit;
            }
        }

        private static void OnAuditButtonClick()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            Console auditableConsole = GetClosestAuditableConsole(localPlayer.GetTruePosition());
            if (auditableConsole != null)
            {
                RpcManager.Instance.SendAuditTask(auditableConsole.transform.position);
            }
        }

        private static Console GetClosestAuditableConsole(Vector2 position)
        {
            Console closestConsole = null;
            float minDistance = float.MaxValue;

            foreach (var console in _allConsoles)
            {
                float distance = Vector2.Distance(position, console.transform.position);
                if (distance < minDistance && distance < 2.0f)
                {
                    if (AuditorManager.Instance.IsTaskAuditable(console.transform.position, out _, out _))
                    {
                        minDistance = distance;
                        closestConsole = console;
                    }
                }
            }
            return closestConsole;
        }
    }
}
