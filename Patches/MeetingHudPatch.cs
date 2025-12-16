using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(MeetingHud))]
    public static class MeetingHudPatch
    {
        private static readonly ConditionalWeakTable<MeetingHud, List<string>> _chatHistories = new ConditionalWeakTable<MeetingHud, List<string>>();
        private static GameObject _echoButton;

        [HarmonyPatch(nameof(MeetingHud.Start))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            if (!_chatHistories.TryGetValue(__instance, out _))
            {
                _chatHistories.Add(__instance, new List<string>());
            }
            else
            {
                _chatHistories.GetOrCreateValue(__instance).Clear();
            }

            if (_echoButton == null)
            {
                _echoButton = new GameObject("EchoButton");
                _echoButton.transform.SetParent(__instance.VoteButton.transform.parent);
                _echoButton.transform.localPosition = new Vector3(-1.5f, 0, 0);
                var renderer = _echoButton.AddComponent<SpriteRenderer>();
                renderer.sprite = __instance.VoteButton.GetComponent<SpriteRenderer>().sprite;
                _echoButton.AddComponent<BoxCollider2D>();
                _echoButton.AddComponent<EchoButton>();
            }
        }

        [HarmonyPatch(nameof(MeetingHud.Update))]
        [HarmonyPostfix]
        public static void UpdatePostfix(MeetingHud __instance)
        {
            var echo = RoleManager.Instance.GetRole(PlayerControl.LocalPlayer.PlayerId) as EchoRole;
            if (echo != null && echo.RecordedChat != null)
            {
                if (!_echoButton.activeSelf) _echoButton.SetActive(true);
            }
            else
            {
                if (_echoButton.activeSelf) _echoButton.SetActive(false);
            }
        }

        [HarmonyPatch(nameof(MeetingHud.AddChat))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance, [HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] string chat)
        {
            if (player != null && !string.IsNullOrEmpty(chat))
            {
                if (_chatHistories.TryGetValue(__instance, out var chatHistory))
                {
                    chatHistory.Add($"{player.name}: {chat}");
                }
            }
        }

        [HarmonyPatch(nameof(MeetingHud.Close))]
        [HarmonyPostfix]
        public static void Postfix(MeetingHud __instance)
        {
            var echo = RoleManager.Instance.GetRole(PlayerControl.LocalPlayer.PlayerId) as EchoRole;
            if (echo != null && __instance.exiledPlayer != null)
            {
                if (_chatHistories.TryGetValue(__instance, out var chatHistory))
                {
                    echo.Record(__instance.exiledPlayer, chatHistory.ToArray());
                }
            }
        }
    }
}
