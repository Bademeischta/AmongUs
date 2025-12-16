using HarmonyLib;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    public static class CustomGameOptions
    {
        public static float JesterChance { get; set; } = 100f;
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
    public static class GameOptionsMenuPatch
    {
        public static NumberOption JesterChanceOption;

        public static void Postfix(GameOptionsMenu __instance)
        {
            JesterChanceOption = Object.Instantiate(__instance.GetComponentsInChildren<NumberOption>()[0], __instance.transform);
            JesterChanceOption.gameObject.name = "JesterChanceOption";

            JesterChanceOption.TitleText.text = "Jester Chance";
            JesterChanceOption.Value = CustomGameOptions.JesterChance; // Use synchronized value
            JesterChanceOption.MinValue = 0f;
            JesterChanceOption.MaxValue = 100f;
            JesterChanceOption.Increment = 5f;
            JesterChanceOption.FormatString = "{0:0}%";

            JesterChanceOption.OnValueChanged.AddListener((System.Action<float>)((value) =>
            {
                CustomGameOptions.JesterChance = value;
            }));
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.ToBytes))]
    public static class GameOptionsDataToBytesPatch
    {
        public static void Postfix(GameOptionsData __instance, ref byte[] __result)
        {
            var writer = new Il2CppSystem.IO.BinaryWriter(new Il2CppSystem.IO.MemoryStream());
            writer.WriteBytes(__result);
            writer.Write(CustomGameOptions.JesterChance);
            __result = ((Il2CppSystem.IO.MemoryStream)writer.BaseStream).ToArray();
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.FromBytes))]
    public static class GameOptionsDataFromBytesPatch
    {
        public static void Postfix(GameOptionsData __instance, byte[] bytes)
        {
            var reader = new Il2CppSystem.IO.BinaryReader(new Il2CppSystem.IO.MemoryStream(bytes));
            reader.BaseStream.Position = __instance.ToBytes().Length;
            CustomGameOptions.JesterChance = reader.ReadSingle();

            // Update the UI if it's open
            if (GameOptionsMenuPatch.JesterChanceOption != null)
            {
                GameOptionsMenuPatch.JesterChanceOption.Value = CustomGameOptions.JesterChance;
            }
        }
    }
}
