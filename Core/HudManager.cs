using UnityEngine;
using TMPro;

namespace MyCustomRolesMod.Core
{
    public class HudManager : MonoBehaviour
    {
        private static HudManager _instance;
        public static HudManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("HudManager");
                    _instance = go.AddComponent<HudManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private TextMeshProUGUI _messageText;
        private float _messageTimer;

        void Update()
        {
            if (_messageTimer > 0)
            {
                _messageTimer -= Time.deltaTime;
                if (_messageTimer <= 0)
                {
                    _messageText.gameObject.SetActive(false);
                }
            }
        }

        public void ShowMessage(string message, float duration = 3f)
        {
            if (_messageText == null)
            {
                var canvas = new GameObject("MessageCanvas");
                var c = canvas.AddComponent<Canvas>();
                c.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

                var textGo = new GameObject("MessageText");
                textGo.transform.SetParent(canvas.transform, false);
                var textRect = textGo.AddComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.5f, 0.8f);
                textRect.anchorMax = new Vector2(0.5f, 0.8f);
                textRect.pivot = new Vector2(0.5f, 0.5f);
                textRect.sizeDelta = new Vector2(800, 100);

                _messageText = textGo.AddComponent<TextMeshProUGUI>();
                _messageText.fontSize = 32;
                _messageText.color = Color.white;
                _messageText.alignment = TextAlignmentOptions.Center;
            }

            _messageText.text = message;
            _messageText.gameObject.SetActive(true);
            _messageTimer = duration;
        }
    }
}
