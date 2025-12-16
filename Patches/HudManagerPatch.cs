using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using TMPro;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerPatch
    {
        private static TextMeshPro _echoText;
        private static TextMeshPro _puppeteerText;
        private static GameObject _puppeteerButton;

        [HarmonyPatch(nameof(HudManager.Start))]
        [HarmonyPostfix]
        public static void StartPostfix(HudManager __instance)
        {
            var O2meter = __instance.O2Meter;

            var echoGo = new GameObject("EchoText");
            echoGo.transform.SetParent(O2meter.transform.parent);
            _echoText = echoGo.AddComponent<TextMeshPro>();
            _echoText.transform.localPosition = new Vector3(0.3f, 1f, 0);
            _echoText.alignment = TextAlignmentOptions.Center;
            _echoText.fontSize = 2;
            _echoText.gameObject.SetActive(false);

            var puppeteerGo = new GameObject("PuppeteerText");
            puppeteerGo.transform.SetParent(O2meter.transform.parent);
            _puppeteerText = puppeteerGo.AddComponent<TextMeshPro>();
            _puppeteerText.transform.localPosition = new Vector3(0.3f, 1f, 0);
            _puppeteerText.alignment = TextAlignmentOptions.Center;
            _puppeteerText.fontSize = 2;
            _puppeteerText.gameObject.SetActive(false);

            _puppeteerButton = new GameObject("PuppeteerButton");
            _puppeteerButton.transform.SetParent(__instance.UseButton.transform.parent);
            _puppeteerButton.transform.localPosition = __instance.KillButton.transform.localPosition;
            var renderer = _puppeteerButton.AddComponent<SpriteRenderer>();
            renderer.sprite = __instance.KillButton.GetComponent<SpriteRenderer>().sprite;
            _puppeteerButton.AddComponent<BoxCollider2D>();
            _puppeteerButton.AddComponent<PuppeteerButton>();
            _puppeteerButton.SetActive(false);
        }

        [HarmonyPatch(nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void UpdatePostfix(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer == null || _echoText == null || _puppeteerText == null)
            {
                if (_echoText != null && _echoText.gameObject.activeSelf) _echoText.gameObject.SetActive(false);
                if (_puppeteerText != null && _puppeteerText.gameObject.activeSelf) _puppeteerText.gameObject.SetActive(false);
                if (_puppeteerButton != null && _puppeteerButton.activeSelf) _puppeteerButton.SetActive(false);
                return;
            }

            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role == null)
            {
                if (_echoText.gameObject.activeSelf) _echoText.gameObject.SetActive(false);
                if (_puppeteerText.gameObject.activeSelf) _puppeteerText.gameObject.SetActive(false);
                if (_puppeteerButton.activeSelf) _puppeteerButton.SetActive(false);
                return;
            }

            if (role is EchoRole echo && echo.RecordedPlayer != null)
            {
                _echoText.text = $"Recorded Chat ({echo.RecordedPlayer.name}):\n{string.Join("\n", echo.RecordedChat)}";
                if (!_echoText.gameObject.activeSelf) _echoText.gameObject.SetActive(true);
            }
            else
            {
                if (_echoText.gameObject.activeSelf) _echoText.gameObject.SetActive(false);
            }

            if (role is PuppeteerRole puppeteer)
            {
                if (!_puppeteerButton.activeSelf) _puppeteerButton.SetActive(true);
                var renderer = _puppeteerButton.GetComponent<SpriteRenderer>();
                renderer.color = puppeteer.IsControlling ? Color.green : (puppeteer.IsSelectingTarget ? Color.yellow : Color.white);

                if (puppeteer.Target != null)
                {
                    _puppeteerText.text = $"Controlling: {puppeteer.Target.name}";
                    if (!_puppeteerText.gameObject.activeSelf) _puppeteerText.gameObject.SetActive(true);
                }
                else
                {
                    if (_puppeteerText.gameObject.activeSelf) _puppeteerText.gameObject.SetActive(false);
                }
            }
            else
            {
                if (_puppeteerButton.activeSelf) _puppeteerButton.SetActive(false);
                if (_puppeteerText.gameObject.activeSelf) _puppeteerText.gameObject.SetActive(false);
            }
        }
    }
}
