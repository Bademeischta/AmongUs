using HarmonyLib;
using UnityEngine;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using TMPro;
using System.Collections.Generic;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class PuppeteerPatch
    {
        private static GameObject _puppeteerButton;
        private static GameObject _puppeteerCanvas;
        private static PlayerControl _selectedTarget;
        private static string _selectedMessage;
        private static List<string> _messages = new List<string> { "I saw them vent!", "They're the killer!", "I'm skipping.", "Vote for me." };

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || RoleManager.Instance.GetRole(localPlayer.PlayerId)?.RoleType != RoleType.Puppeteer)
            {
                if (_puppeteerButton != null) _puppeteerButton.SetActive(false);
                if (_puppeteerCanvas != null) _puppeteerCanvas.SetActive(false);
                return;
            }
            if (localPlayer.Data.IsDead)
            {
                if (_puppeteerButton != null) _puppeteerButton.SetActive(false);
                if (_puppeteerCanvas != null) _puppeteerCanvas.SetActive(false);
                return;
            }

            if (_puppeteerButton == null && __instance.UseButton != null)
            {
                _puppeteerButton = Object.Instantiate(__instance.UseButton.gameObject, __instance.UseButton.transform.parent);
                _puppeteerButton.transform.localPosition = new Vector3(2.5f, 1.5f, 0); // Position it somewhere reasonable
                _puppeteerButton.name = "PuppeteerButton";
                var renderer = _puppeteerButton.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = __instance.KillButton.GetComponent<SpriteRenderer>().sprite; // Use kill button sprite for now
                    renderer.color = Color.magenta;
                }

                var passiveButton = _puppeteerButton.GetComponent<PassiveButton>();
                if (passiveButton != null)
                {
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener(OnPuppeteerButtonClick);
                }

                var text = _puppeteerButton.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Project Voice";
                }
            }
            if (_puppeteerButton != null)
            {
                _puppeteerButton.SetActive(!localPlayer.Data.IsDead && _puppeteerCanvas?.activeSelf != true);
            }
        }
        private static void OnPuppeteerButtonClick()
        {
            if (_puppeteerCanvas == null)
            {
                _puppeteerCanvas = new GameObject("PuppeteerCanvas");
                var canvas = _puppeteerCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _puppeteerCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                var panel = new GameObject("Panel");
                panel.transform.SetParent(_puppeteerCanvas.transform, false);
                var panelRect = panel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(600, 400);
                panel.AddComponent<UnityEngine.UI.Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

                var y = 150f;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == PlayerControl.LocalPlayer.PlayerId) continue;
                    var playerButtonGo = new GameObject($"PlayerButton_{player.PlayerId}");
                    playerButtonGo.transform.SetParent(panel.transform, false);
                    var buttonRect = playerButtonGo.AddComponent<RectTransform>();
                    buttonRect.sizeDelta = new Vector2(180, 40);
                    buttonRect.anchoredPosition = new Vector2(-150, y);
                    playerButtonGo.AddComponent<UnityEngine.UI.Image>().color = Palette.PlayerColors[player.Data.ColorId];
                    var button = playerButtonGo.AddComponent<UnityEngine.UI.Button>();
                    var textGo = new GameObject("Text");
                    textGo.transform.SetParent(playerButtonGo.transform, false);
                    var textRect = textGo.AddComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    var text = textGo.AddComponent<TextMeshProUGUI>();
                    text.text = player.Data.PlayerName;
                    text.color = Color.white;
                    text.alignment = TextAlignmentOptions.Center;
                    button.onClick.AddListener(() => _selectedTarget = player);
                    y -= 50f;
                }

                y = 150f;
                foreach (var message in _messages)
                {
                    var messageButtonGo = new GameObject($"MessageButton_{message}");
                    messageButtonGo.transform.SetParent(panel.transform, false);
                    var buttonRect = messageButtonGo.AddComponent<RectTransform>();
                    buttonRect.sizeDelta = new Vector2(180, 40);
                    buttonRect.anchoredPosition = new Vector2(150, y);
                    messageButtonGo.AddComponent<UnityEngine.UI.Image>().color = Color.gray;
                    var button = messageButtonGo.AddComponent<UnityEngine.UI.Button>();
                    var textGo = new GameObject("Text");
                    textGo.transform.SetParent(messageButtonGo.transform, false);
                    var textRect = textGo.AddComponent<RectTransform>();
                    textRect.anchorMin = Vector2.zero;
                    textRect.anchorMax = Vector2.one;
                    var text = textGo.AddComponent<TextMeshProUGUI>();
                    text.text = message;
                    text.color = Color.white;
                    text.alignment = TextAlignmentOptions.Center;
                    button.onClick.AddListener(() => _selectedMessage = message);
                    y -= 50f;
                }

                var sendButtonGo = new GameObject("SendButton");
                sendButtonGo.transform.SetParent(panel.transform, false);
                var sendButtonRect = sendButtonGo.AddComponent<RectTransform>();
                sendButtonRect.sizeDelta = new Vector2(100, 40);
                sendButtonRect.anchoredPosition = new Vector2(0, -150);
                sendButtonGo.AddComponent<UnityEngine.UI.Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
                var sendButton = sendButtonGo.AddComponent<UnityEngine.UI.Button>();
                var sendButtonTextGo = new GameObject("Text");
                sendButtonTextGo.transform.SetParent(sendButtonGo.transform, false);
                var sendButtonTextRect = sendButtonTextGo.AddComponent<RectTransform>();
                sendButtonTextRect.anchorMin = Vector2.zero;
                sendButtonTextRect.anchorMax = Vector2.one;
                var sendButtonText = sendButtonTextGo.AddComponent<TextMeshProUGUI>();
                sendButtonText.text = "Send";
                sendButtonText.color = Color.white;
                sendButtonText.alignment = TextAlignmentOptions.Center;
                sendButton.onClick.AddListener(OnSendButtonClick);
            }
            _puppeteerCanvas.SetActive(true);
        }

        private static void OnSendButtonClick()
        {
            if (_selectedTarget != null && !string.IsNullOrEmpty(_selectedMessage))
            {
                RpcManager.Instance.SendSetPuppeteerForcedMessage(_selectedTarget.PlayerId, _selectedMessage);
                if (_puppeteerCanvas != null)
                {
                    _puppeteerCanvas.SetActive(false);
                }
                _selectedTarget = null;
                _selectedMessage = null;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static void Postfix(MeetingHud __instance)
        {
            foreach (var player in __instance.playerStates)
            {
                var message = PuppeteerManager.Instance.GetForcedMessage(player.TargetPlayerId);
                if (!string.IsNullOrEmpty(message))
                {
                    player.SetChatBubbleText(message);
                    if (AmongUsClient.Instance.AmHost)
                    {
                        RpcManager.Instance.SendSetPuppeteerForcedMessage(player.TargetPlayerId, null);
                    }
                }
            }
        }
    }
}