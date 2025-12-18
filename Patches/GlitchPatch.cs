using HarmonyLib;
using UnityEngine;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using TMPro;
using System.Linq;
using System.Collections.Generic;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class GlitchPatch
    {
        private static GameObject _glitchButton;
        public static List<Minigame> _corruptibleSystems;
        public static Dictionary<Minigame, TaskTypes> _minigameToTaskTypeMap;

        public static void ClearCorruptibleSystems()
        {
            _corruptibleSystems?.Clear();
            _minigameToTaskTypeMap?.Clear();
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
        public static void Postfix(ShipStatus __instance)
        {
            _corruptibleSystems = new List<Minigame>();
            _minigameToTaskTypeMap = new Dictionary<Minigame, TaskTypes>();

            foreach (var minigame in __instance.allMinigames)
            {
                if (minigame.TaskType != TaskTypes.None)
                {
                    _corruptibleSystems.Add(minigame);
                    _minigameToTaskTypeMap[minigame] = minigame.TaskType;
                }
            }
        }


        [HarmonyPatch(typeof(global::HudManager), nameof(global::HudManager.Update))]
        public static void Postfix(global::HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || RoleManager.Instance.GetRole(localPlayer.PlayerId)?.RoleType != RoleType.Glitch)
            {
                if (_glitchButton != null) _glitchButton.SetActive(false);
                return;
            }

            if (localPlayer.Data.IsDead)
            {
                if (_glitchButton != null) _glitchButton.SetActive(false);
                return;
            }

            if (_glitchButton == null && __instance.UseButton != null)
            {
                _glitchButton = Object.Instantiate(__instance.UseButton.gameObject, __instance.UseButton.transform.parent);
                _glitchButton.transform.localPosition = new Vector3(2.5f, 2.5f, 0); // Position it somewhere reasonable
                _glitchButton.name = "GlitchButton";
                var renderer = _glitchButton.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.sprite = __instance.KillButton.GetComponent<SpriteRenderer>().sprite; // Use kill button sprite for now
                    renderer.color = Color.green;
                }

                var passiveButton = _glitchButton.GetComponent<PassiveButton>();
                if (passiveButton != null)
                {
                    passiveButton.OnClick.RemoveAllListeners();
                    passiveButton.OnClick.AddListener(OnGlitchButtonClick);
                }

                var text = _glitchButton.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = "Corrupt";
                }
            }
            if (_glitchButton != null)
            {
                var usable = FindClosestCorruptibleSystem();
                _glitchButton.SetActive(!localPlayer.Data.IsDead && usable != null);
            }
        }

        private static void OnGlitchButtonClick()
        {
            var minigame = FindClosestCorruptibleSystem();
            if (minigame != null && _minigameToTaskTypeMap.TryGetValue(minigame, out var taskType))
            {
                RpcManager.Instance.SendSetGlitchCorruptedSystem((int)taskType);
            }
        }

        private static Minigame FindClosestCorruptibleSystem()
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || _corruptibleSystems == null) return null;

            Minigame closest = null;
            var minDistance = float.MaxValue;

            foreach (var minigame in _corruptibleSystems)
            {
                var distance = Vector2.Distance(localPlayer.GetTruePosition(), minigame.transform.position);
                if (distance < 2f && distance < minDistance) // 2f is a reasonable usable distance
                {
                    minDistance = distance;
                    closest = minigame;
                }
            }
            return closest;
        }

        [HarmonyPatch(typeof(global::PlayerTask), nameof(global::PlayerTask.Update))]
        public static void Postfix(global::PlayerTask __instance)
        {
            if (__instance.Owner == PlayerControl.LocalPlayer && GlitchManager.Instance.IsSystemCorrupted((int)__instance.TaskType))
            {
                // Simple example: reverse task progress
                if (__instance.taskStep > 0)
                {
                    __instance.taskStep = Mathf.Max(0, __instance.taskStep - 1);
                }
            }
        }
    }
}
