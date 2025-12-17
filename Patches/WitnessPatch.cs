using HarmonyLib;
using UnityEngine;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using TMPro;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class WitnessPatch
    {
        private static GameObject _testimonyButton;
        private static GameObject _testimonyInputCanvas;
        private static TMP_InputField _inputField;
        private static string _testimony = "";

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || RoleManager.Instance.GetRole(localPlayer.PlayerId)?.RoleType != RoleType.Witness)
            {
                if (_testimonyButton != null) _testimonyButton.SetActive(false);
                if (_testimonyInputCanvas != null) _testimonyInputCanvas.SetActive(false);
                return;
            }

            if (localPlayer.Data.IsDead)
            {
                if (_testimonyButton != null) _testimonyButton.SetActive(false);
                if (_testimonyInputCanvas != null) _testimonyInputCanvas.SetActive(false);
                return;
            }

            if (_testimonyButton == null && __instance.UseButton != null)
            {
                _testimonyButton = Object.Instantiate(__instance.UseButton.gameObject, __instance.UseButton.transform.parent);
                _testimonyButton.transform.localPosition = new Vector3(2.5f, 0.5f, 0); // Position it somewhere reasonable
                _testimonyButton.name = "TestimonyButton";
                var renderer = _testimonyButton.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = __instance.KillButton.GetComponent<SpriteRenderer>().sprite; // Use kill button sprite for now
                }

                var passiveButton = _testimonyButton.GetComponent<PassiveButton>();
                if (passiveButton != null)
                {
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener(OnTestimonyButtonClick);
                }

                var text = _testimonyButton.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Write Testimony";
                }
            }
            if (_testimonyButton != null)
            {
                _testimonyButton.SetActive(!localPlayer.Data.IsDead && _testimonyInputCanvas?.activeSelf != true);
            }
        }

        private static void OnTestimonyButtonClick()
        {
            if (_testimonyInputCanvas == null)
            {
                // Create canvas and all UI elements
                _testimonyInputCanvas = new GameObject("TestimonyInputCanvas");
                var canvas = _testimonyInputCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = _testimonyInputCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

                var panel = new GameObject("Panel");
                panel.transform.SetParent(_testimonyInputCanvas.transform, false);
                var panelRect = panel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(500, 200);
                panel.AddComponent<UnityEngine.UI.Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

                var inputFieldGo = new GameObject("InputField");
                inputFieldGo.transform.SetParent(panel.transform, false);
                var inputFieldRect = inputFieldGo.AddComponent<RectTransform>();
                inputFieldRect.sizeDelta = new Vector2(480, 100);
                inputFieldRect.anchoredPosition = new Vector2(0, 20);
                var inputFieldImage = inputFieldGo.AddComponent<UnityEngine.UI.Image>();
                inputFieldImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);

                _inputField = inputFieldGo.AddComponent<TMP_InputField>();

                var textArea = new GameObject("TextArea");
                textArea.transform.SetParent(inputFieldGo.transform, false);
                var textAreaRect = textArea.AddComponent<RectTransform>();
                textAreaRect.anchorMin = Vector2.zero;
                textAreaRect.anchorMax = Vector2.one;
                textAreaRect.offsetMin = new Vector2(10, 10);
                textAreaRect.offsetMax = new Vector2(-10, -10);

                var textGo = new GameObject("Text");
                textGo.transform.SetParent(textArea.transform, false);
                var textRect = textGo.AddComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                var text = textGo.AddComponent<TextMeshProUGUI>();
                text.fontSize = 20;
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.TopLeft;

                _inputField.textComponent = text;
                _inputField.textViewport = textAreaRect;

                var placeholderGo = new GameObject("Placeholder");
                placeholderGo.transform.SetParent(inputFieldGo.transform, false);
                var placeholderRect = placeholderGo.AddComponent<RectTransform>();
                placeholderRect.anchorMin = Vector2.zero;
                placeholderRect.anchorMax = Vector2.one;
                placeholderRect.offsetMin = new Vector2(10, 10);
                placeholderRect.offsetMax = new Vector2(-10, -10);
                var placeholder = placeholderGo.AddComponent<TextMeshProUGUI>();
                placeholder.text = "Enter your testimony...";
                placeholder.fontSize = 20;
                placeholder.color = Color.gray;
                placeholder.fontStyle = FontStyles.Italic;
                _inputField.placeholder = placeholder;

                var saveButtonGo = new GameObject("SaveButton");
                saveButtonGo.transform.SetParent(panel.transform, false);
                var saveButtonRect = saveButtonGo.AddComponent<RectTransform>();
                saveButtonRect.sizeDelta = new Vector2(100, 40);
                saveButtonRect.anchoredPosition = new Vector2(0, -70);
                saveButtonGo.AddComponent<UnityEngine.UI.Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
                var saveButton = saveButtonGo.AddComponent<UnityEngine.UI.Button>();

                var saveButtonTextGo = new GameObject("Text");
                saveButtonTextGo.transform.SetParent(saveButtonGo.transform, false);
                var saveButtonTextRect = saveButtonTextGo.AddComponent<RectTransform>();
                saveButtonTextRect.anchorMin = Vector2.zero;
                saveButtonTextRect.anchorMax = Vector2.one;
                var saveButtonText = saveButtonTextGo.AddComponent<TextMeshProUGUI>();
                saveButtonText.text = "Save";
                saveButtonText.fontSize = 24;
                saveButtonText.color = Color.white;
                saveButtonText.alignment = TextAlignmentOptions.Center;
                saveButton.onClick.AddListener(OnSaveButtonClick);
            }
            _testimonyInputCanvas.SetActive(true);
            _inputField.text = _testimony;
        }

        private static void OnSaveButtonClick()
        {
            _testimony = _inputField.text;
            RpcManager.Instance.SendSetWitnessTestimony(PlayerControl.LocalPlayer.PlayerId, _testimony);
            if (_testimonyInputCanvas != null)
            {
                _testimonyInputCanvas.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null) return;

            var deadPlayer = __instance.target;
            if (deadPlayer == null) return;

            var role = RoleManager.Instance.GetRole(deadPlayer.PlayerId);
            if (role?.RoleType == RoleType.Witness)
            {
                var testimony = WitnessManager.Instance.GetTestimony(deadPlayer.PlayerId);
                if (!string.IsNullOrEmpty(testimony))
                {
                    var chatBubble = __instance.discussionPlayerStates[deadPlayer.PlayerId];
                    if (chatBubble != null)
                    {
                        chatBubble.SetChatBubbleText($"Testimony: {testimony}");
                    }
                }
            }
        }
    }
}