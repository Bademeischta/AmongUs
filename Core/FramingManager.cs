using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyCustomRolesMod.Networking;

namespace MyCustomRolesMod.Core
{
    public class FramingManager : MonoBehaviour
    {
        private static FramingManager _instance;
        public static FramingManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("FramingManager");
                    _instance = go.AddComponent<FramingManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private GameObject _framingCanvas;
        private List<PlayerControl> _playersToFrame;

        public void ShowFramingUI(List<PlayerControl> players)
        {
            _playersToFrame = players;
            if (_framingCanvas == null)
            {
                _framingCanvas = new GameObject("FramingCanvas");
                var canvas = _framingCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = _framingCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

                var panel = new GameObject("Panel");
                panel.transform.SetParent(_framingCanvas.transform, false);
                var panelRect = panel.AddComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.5f, 0.5f);
                panelRect.anchorMax = new Vector2(0.5f, 0.5f);
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                panelRect.sizeDelta = new Vector2(300, 400);
                panel.AddComponent<UnityEngine.UI.Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

                var titleGo = new GameObject("Title");
                titleGo.transform.SetParent(panel.transform, false);
                var titleRect = titleGo.AddComponent<RectTransform>();
                titleRect.anchorMin = new Vector2(0.5f, 0.9f);
                titleRect.anchorMax = new Vector2(0.5f, 0.9f);
                titleRect.pivot = new Vector2(0.5f, 0.5f);
                titleRect.sizeDelta = new Vector2(280, 50);
                var title = titleGo.AddComponent<TextMeshProUGUI>();
                title.text = "Frame a Player";
                title.fontSize = 28;
                title.color = Color.white;
                title.alignment = TextAlignmentOptions.Center;
            }

            // Clear old buttons
            foreach (Transform child in _framingCanvas.transform.Find("Panel"))
            {
                if (child.name.StartsWith("PlayerButton"))
                {
                    Destroy(child.gameObject);
                }
            }

            for (int i = 0; i < _playersToFrame.Count; i++)
            {
                var player = _playersToFrame[i];
                var buttonGo = new GameObject("PlayerButton" + i);
                buttonGo.transform.SetParent(_framingCanvas.transform.Find("Panel"), false);
                var buttonRect = buttonGo.AddComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(250, 40);
                buttonRect.anchoredPosition = new Vector2(0, 120 - (i * 50));
                buttonGo.AddComponent<UnityEngine.UI.Image>().color = new Color(0.3f, 0.3f, 0.3f, 1f);
                var button = buttonGo.AddComponent<UnityEngine.UI.Button>();

                var buttonTextGo = new GameObject("Text");
                buttonTextGo.transform.SetParent(buttonGo.transform, false);
                var buttonTextRect = buttonTextGo.AddComponent<RectTransform>();
                buttonTextRect.anchorMin = Vector2.zero;
                buttonTextRect.anchorMax = Vector2.one;
                var buttonText = buttonTextGo.AddComponent<TextMeshProUGUI>();
                buttonText.text = player.Data.PlayerName;
                buttonText.fontSize = 24;
                buttonText.color = Color.white;
                buttonText.alignment = TextAlignmentOptions.Center;

                var playerId = player.PlayerId;
                button.onClick.AddListener(() => OnPlayerSelected(playerId));
            }

            _framingCanvas.SetActive(true);
        }

        private void OnPlayerSelected(byte playerId)
        {
            RpcManager.Instance.SendFramePlayer(playerId);
            _framingCanvas.SetActive(false);
        }
    }
}
