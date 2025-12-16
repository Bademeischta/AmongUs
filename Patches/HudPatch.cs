using HarmonyLib;
using MyCustomRolesMod.Core;
using TMPro;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    public static class HudPatch
    {
        private static TextMeshPro _roleText;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
        public static class HudManagerStartPatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (_roleText != null) Object.Destroy(_roleText.gameObject);

                var roleTextGo = new GameObject("CustomRoleText");
                roleTextGo.transform.SetParent(__instance.transform);

                int uiLayer = LayerMask.NameToLayer("UI");
                roleTextGo.layer = (uiLayer == -1) ? 5 : uiLayer;

                _roleText = roleTextGo.AddComponent<TextMeshPro>();
                _roleText.font = __instance.TaskText.font;
                _roleText.fontSize = 2.0f;
                _roleText.alignment = TextAlignmentOptions.TopLeft;

                var rectTransform = _roleText.rectTransform;
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.anchoredPosition = new Vector2(1.2f, -0.2f);
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.OnDestroy))]
        public static class HudManagerOnDestroyPatch
        {
            public static void Postfix()
            {
                if (_roleText != null) Object.Destroy(_roleText.gameObject);
                _roleText = null;
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlFixedUpdatePatch
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance != PlayerControl.LocalPlayer) return;

                var role = RoleManager.Instance.GetRole(__instance.PlayerId);
                if (role != null)
                {
                    __instance.nameText.color = role.Color;
                    if (_roleText != null)
                        _roleText.text = $"Rolle: <color=#{ColorUtility.ToHtmlStringRGB(role.Color)}>{role.Name}</color>";
                }
                else
                {
                    __instance.nameText.color = Color.white;
                    if (_roleText != null) _roleText.text = "";
                }
            }
        }
    }
}
