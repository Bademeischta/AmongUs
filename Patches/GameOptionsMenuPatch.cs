using HarmonyLib;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
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
            if (_jesterChanceOption != null) Object.Destroy(_jesterChanceOption.gameObject);

            _jesterChanceOption = Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[0], __instance.transform);
            _jesterChanceOption.gameObject.name = "JesterChanceOption";

            _jesterChanceOption.TitleText.text = "Jester Chance";
            _jesterChanceOption.Value = ModPlugin.ModConfig.JesterChance.Value;
            _jesterChanceOption.MinValue = 0f;
            _jesterChanceOption.MaxValue = 100f;
            _jesterChanceOption.Increment = 5f;
            _jesterChanceOption.FormatString = "{0:0}%";

            _jesterChanceOption.OnValueChanged.AddListener((System.Action<float>)((value) =>
            {
                ModPlugin.ModConfig.JesterChance.Value = value;
                if (AmongUsClient.Instance.AmHost)
                {
                    var packet = new OptionsPacket { JesterChance = value };
                    var writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SyncOptions);
                    packet.Serialize(writer);
                    writer.EndMessage();
                    RpcManager.Instance.Send(writer);
                }
            }));
        }

        public static void UpdateUI()
        {
            if (_jesterChanceOption != null)
            {
                _jesterChanceOption.Value = ModPlugin.ModConfig.JesterChance.Value;
            }
        }
    }
}
