using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;
using TMPro;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class PropagatorPatch
    {
        private static TextMeshPro _infectionTimerText;

        [HarmonyPatch(typeof(global::HudManager), nameof(global::HudManager.Update))]
        public static void Postfix(global::HudManager __instance)
        {
            if (PlayerControl.LocalPlayer == null) return;

            if (_infectionTimerText == null)
            {
                var timerGo = new GameObject("InfectionTimerText");
                timerGo.transform.SetParent(__instance.transform, false);
                var timerRect = timerGo.AddComponent<RectTransform>();
                timerRect.anchorMin = new Vector2(0.5f, 0.2f);
                timerRect.anchorMax = new Vector2(0.5f, 0.2f);
                timerRect.pivot = new Vector2(0.5f, 0.5f);
                timerRect.sizeDelta = new Vector2(400, 50);

                _infectionTimerText = timerGo.AddComponent<TextMeshProUGUI>();
                _infectionTimerText.fontSize = 24;
                _infectionTimerText.color = Color.red;
                _infectionTimerText.alignment = TextAlignmentOptions.Center;
            }

            var localPlayerId = PlayerControl.LocalPlayer.PlayerId;
            var infectedPlayerId = PropagatorManager.Instance.InfectedPlayerId;

            if (localPlayerId == infectedPlayerId)
            {
                _infectionTimerText.gameObject.SetActive(true);
                var timeRemaining = 20f - (Time.time - PropagatorManager.Instance.InfectionTime);
                _infectionTimerText.text = $"Infected! Pass it on! {timeRemaining:F1}s";
            }
            else
            {
                _infectionTimerText.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(global::KillButtonManager), nameof(global::KillButtonManager.Update))]
        public static void Postfix(global::KillButtonManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role?.RoleType == RoleType.Propagator)
            {
                bool canInfect = PropagatorManager.Instance.InfectedPlayerId == byte.MaxValue;
                __instance.KillButton.gameObject.SetActive(true);
                __instance.KillButton.GetComponentInChildren<TextMeshPro>().text = "Infect";
                __instance.isCoolingDown = !canInfect;

                PlayerControl target = __instance.currentTarget;
                if (target != null && canInfect)
                {
                    __instance.KillButton.GetComponent<SpriteRenderer>().color = Color.white;
                    if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Kill"))
                    {
                        RpcManager.Instance.SendInfectPlayer(target.PlayerId);
                    }
                }
                else
                {
                    __instance.KillButton.GetComponent<SpriteRenderer>().color = Color.gray;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static void Postfix(PlayerControl __instance)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                PropagatorManager.Instance.Update();
            }
        }
    }
}
