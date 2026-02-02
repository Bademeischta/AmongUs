using HarmonyLib;
using UnityEngine;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using TMPro;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class SolipsistPatch
    {
        private static GameObject _censureButton;
        private static DeadBody _nearestBody;
        private static PlayerControl _nearestPlayer;

        [HarmonyPatch(typeof(global::HudManager), nameof(global::HudManager.Update))]
        public static void Postfix(global::HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null) return;

            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);

            // Handle Censure Button for Solipsist
            if (role?.RoleType == RoleType.Solipsist && !localPlayer.Data.IsDead)
            {
                if (_censureButton == null && __instance.KillButton != null)
                {
                    _censureButton = Object.Instantiate(__instance.KillButton.gameObject, __instance.KillButton.transform.parent);
                    _censureButton.transform.localPosition = new Vector3(2.5f, 1.5f, 0);
                    _censureButton.name = "CensureButton";

                    var renderer = _censureButton.GetComponent<SpriteRenderer>();
                    if (renderer != null) renderer.color = new Color(1f, 0.5f, 0f); // Orange

                    var passiveButton = _censureButton.GetComponent<PassiveButton>();
                    if (passiveButton != null)
                    {
                        passiveButton.OnClick.RemoveAllListeners();
                        passiveButton.OnClick.AddListener((System.Action)OnCensureButtonClick);
                    }

                    var text = _censureButton.GetComponentInChildren<TextMeshPro>();
                    if (text != null) text.text = "Censure";
                }

                if (_censureButton != null)
                {
                    FindTargets(localPlayer);
                    _censureButton.SetActive(_nearestBody != null && _nearestPlayer != null);
                }
            }
            else
            {
                if (_censureButton != null) _censureButton.SetActive(false);
            }

            // Handle perceptual effects for the local player
            UpdateLocalPerception(localPlayer, __instance);
        }

        private static void FindTargets(PlayerControl localPlayer)
        {
            var truePos = localPlayer.GetTruePosition();

            // Find nearest body
            _nearestBody = Object.FindObjectsOfType<DeadBody>()
                .OrderBy(b => Vector2.Distance(truePos, b.transform.position))
                .FirstOrDefault(b => Vector2.Distance(truePos, b.transform.position) < 3.0f);

            // Find nearest other player
            _nearestPlayer = PlayerControl.AllPlayerControls
                .Where(p => p.PlayerId != localPlayer.PlayerId && !p.Data.IsDead)
                .OrderBy(p => Vector2.Distance(truePos, p.GetTruePosition()))
                .FirstOrDefault(p => Vector2.Distance(truePos, p.GetTruePosition()) < 3.0f);
        }

        private static void OnCensureButtonClick()
        {
            if (_nearestBody != null && _nearestPlayer != null)
            {
                RpcManager.Instance.SendSetCensoredObject(_nearestPlayer.PlayerId, _nearestBody.NetId);
                ModPlugin.Logger.LogInfo($"[Solipsist] Censured body {_nearestBody.NetId} for player {_nearestPlayer.Data.PlayerName}");
            }
        }

        private static float _lastBodyUpdate;
        private static DeadBody[] _cachedBodies;

        private static void UpdateLocalPerception(PlayerControl localPlayer, global::HudManager hud)
        {
            // Hide censured bodies
            if (Time.time - _lastBodyUpdate > 0.5f)
            {
                _cachedBodies = Object.FindObjectsOfType<DeadBody>();
                _lastBodyUpdate = Time.time;
            }

            if (_cachedBodies == null) return;

            bool nearestCensured = false;
            DeadBody closestToLocal = null;
            float minDist = float.MaxValue;

            foreach (var body in _cachedBodies)
            {
                bool isCensured = SolipsistManager.Instance.IsCensored(localPlayer.PlayerId, body.NetId);

                var renderer = body.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.enabled = !isCensured;
                }

                float dist = Vector2.Distance(localPlayer.GetTruePosition(), body.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestToLocal = body;
                    if (isCensured) nearestCensured = true;
                }
            }

            // Disable report button if nearest body is censured
            if (hud.ReportButton != null && minDist < 2.0f && nearestCensured)
            {
                // Note: We can't easily disable the button's internal logic without a prefix patch,
                // but we can make it look disabled and hope the game doesn't trigger it if renderer is off.
                // Actually, let's patch ReportButton.DoClick as well.
                hud.ReportButton.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            }
        }

        [HarmonyPatch(typeof(global::ReportButton), nameof(global::ReportButton.DoClick))]
        public static class ReportButtonDoClickPatch
        {
            public static bool Prefix()
            {
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;

                var bodies = Object.FindObjectsOfType<DeadBody>();
                var nearest = bodies
                    .OrderBy(b => Vector2.Distance(localPlayer.GetTruePosition(), b.transform.position))
                    .FirstOrDefault();

                if (nearest != null && Vector2.Distance(localPlayer.GetTruePosition(), nearest.transform.position) < 2.0f)
                {
                    if (SolipsistManager.Instance.IsCensored(localPlayer.PlayerId, nearest.NetId))
                    {
                        return false; // Block reporting
                    }
                }
                return true;
            }
        }
    }
}
