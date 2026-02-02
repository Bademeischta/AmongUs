using HarmonyLib;
using UnityEngine;
using MyCustomRolesMod.Core;
using TMPro;
using System.Text;
using System;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class DendrochronologistPatch
    {
        private static GameObject _extractionButton;
        private static GameObject _ageButton;
        public static float LastMessageTime;
        public static string TempMessage = "";

        [HarmonyPatch(typeof(global::HudManager), nameof(global::HudManager.Update))]
        public static void Postfix(global::HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || RoleManager.Instance.GetRole(localPlayer.PlayerId)?.RoleType != RoleType.Dendrochronologist)
            {
                if (_extractionButton != null) _extractionButton.SetActive(false);
                if (_ageButton != null) _ageButton.SetActive(false);
                return;
            }

            if (localPlayer.Data.IsDead)
            {
                if (_extractionButton != null) _extractionButton.SetActive(false);
                if (_ageButton != null) _ageButton.SetActive(false);
                return;
            }

            if (_extractionButton == null && __instance.ReportButton != null)
            {
                _extractionButton = UnityEngine.Object.Instantiate(__instance.ReportButton.gameObject, __instance.ReportButton.transform.parent);
                _extractionButton.transform.localPosition = new Vector3(-2.5f, 0.5f, 0);
                _extractionButton.name = "ExtractionButton";

                var passiveButton = _extractionButton.GetComponent<PassiveButton>();
                if (passiveButton != null)
                {
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener((Action)OnExtractionButtonClick);
                }

                var text = _extractionButton.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Extract Core";
                }
            }

            if (_ageButton == null && __instance.ReportButton != null)
            {
                _ageButton = UnityEngine.Object.Instantiate(__instance.ReportButton.gameObject, __instance.ReportButton.transform.parent);
                _ageButton.transform.localPosition = new Vector3(-2.5f, 1.5f, 0);
                _ageButton.name = "AgeButton";

                var passiveButton = _ageButton.GetComponent<PassiveButton>();
                if (passiveButton != null)
                {
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener((Action)OnAgeButtonClick);
                }

                var text = _ageButton.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Sample Age";
                }
            }

            if (_extractionButton != null) _extractionButton.SetActive(true);
            if (_ageButton != null) _ageButton.SetActive(true);

            // Display temporary message if set
            if (!string.IsNullOrEmpty(TempMessage) && Time.time - LastMessageTime < 5.0f)
            {
                // We could use HudManager.Instance.DisplayNotice here if we knew the exact name,
                // but for now we rely on the player reading the HUD or we can log it.
                // In a real mod, this would be a UI element.
            }
            else
            {
                TempMessage = "";
            }
        }

        private static void OnExtractionButtonClick()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            string room = DendrochronologistManager.Instance.GetCurrentRoom(localPlayer);
            var history = DendrochronologistManager.Instance.GetHistory(room);

            if (history.Count == 0)
            {
                ShowMessage("No residue found in this room.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"Residue in {room}: ");
            for (int i = history.Count - 1; i >= 0; i--)
            {
                var visit = history[i];
                sb.Append($"<color=#{ColorUtility.ToHtmlStringRGB(visit.Color)}>●</color> ");
            }
            ShowMessage(sb.ToString());
        }

        private static void OnAgeButtonClick()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            string room = DendrochronologistManager.Instance.GetCurrentRoom(localPlayer);
            var history = DendrochronologistManager.Instance.GetHistory(room);

            if (history.Count == 0)
            {
                ShowMessage("Room is pristine.");
                return;
            }

            var lastVisit = history[history.Count - 1];
            float age = Time.time - lastVisit.Timestamp;
            ShowMessage($"Last entry: {age:F1}s ago.");
        }

        private static void ShowMessage(string message)
        {
            TempMessage = message;
            LastMessageTime = Time.time;
            ModPlugin.Logger.LogInfo($"[Dendrochronologist] {message}");
        }
    }
}
