using HarmonyLib;
using MyCustomRolesMod.Management;
using TMPro;
using UnityEngine;
using MyCustomRolesMod.Roles;

namespace MyCustomRolesMod.Patches
{
    public static class UiPatch
    {
        private static TextMeshPro roleText;

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class PlayerControlFixedUpdatePatch
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance != PlayerControl.LocalPlayer) return;

                var role = RoleManager.GetRole(__instance);
                if (role != null)
                {
                    __instance.nameText.color = role.Color;
                }
                else
                {
                    // Reset to default color if no custom role
                    __instance.nameText.color = Color.white;
                }
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManagerUpdatePatch
        {
            public static void Postfix(HudManager __instance)
            {
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null)
                {
                    if (roleText != null) Object.Destroy(roleText.gameObject);
                    return;
                }

                var role = RoleManager.GetRole(localPlayer);
                if (role is JesterRole jesterRole)
                {
                    if (roleText == null)
                    {
                        var textGo = new GameObject("RoleText");
                        textGo.transform.SetParent(__instance.transform);
                        textGo.layer = 5; // UI layer

                        roleText = textGo.AddComponent<TextMeshPro>();
                        roleText.font = __instance.TaskText.font;
                        roleText.fontSize = 2.5f;
                        roleText.alignment = TextAlignmentOptions.Center;
                        roleText.rectTransform.anchoredPosition = new Vector2(0, -2.5f);
                    }
                    roleText.text = $"Rolle: {jesterRole.Name}";
                    roleText.color = jesterRole.Color;
                }
                else
                {
                    if (roleText != null) Object.Destroy(roleText.gameObject);
                }
            }
        }
    }
}
