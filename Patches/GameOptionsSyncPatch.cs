using HarmonyLib;
using MyCustomRolesMod.Management;
using UnityEngine;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class GameOptionsMenuPatch
    {
        private static NumberOption _jesterChanceOption;

        public static void Postfix(GameOptionsMenu __instance)
        {
            _jesterChanceOption = Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[0], __instance.transform);
            _jesterChanceOption.gameObject.name = "JesterChanceOption";

            _jesterChanceOption.TitleText.text = "Jester Chance";
            _jesterChanceOption.Value = CustomGameOptions.JesterChance;
            _jesterChanceOption.MinValue = 0f;
            _jesterChanceOption.MaxValue = 100f;
            _jesterChanceOption.Increment = 5f;
            _jesterChanceOption.FormatString = "{0:0}%";

            _jesterChanceOption.OnValueChanged.AddListener((System.Action<float>)((value) =>
            {
                CustomGameOptions.JesterChance = value;
                if (AmongUsClient.Instance.AmHost)
                {
                    SendOptions();
                }
            }));
        }

        public static void UpdateUI()
        {
            if (_jesterChanceOption != null)
            {
                _jesterChanceOption.Value = CustomGameOptions.JesterChance;
            }
        }

        private static void SendOptions()
        {
            ModPlugin.Logger.LogInfo($"Host sending game options: JesterChance={CustomGameOptions.JesterChance}%");
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage(NetworkManager.GameOptionsSyncRpcId);
            writer.Write(CustomGameOptions.JesterChance);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer);
        }
    }
}
