using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using MyCustomRolesMod.Config;
using MyCustomRolesMod.Networking;
using System.Reflection;
using UnityEngine;

namespace MyCustomRolesMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ModPlugin : BasePlugin
    {
        public static ManualLogSource Logger { get; private set; }
        public static ModConfig ModConfig { get; private set; }
        private readonly Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public override void Load()
        {
            Logger = Log;
            ModConfig = new ModConfig(Config);

            // Register RpcManager to receive Unity's Update events.
            AddComponent<RpcManager>();

            _harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} loaded.");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.example.mycustomrolesmod.paranoid";
        public const string PLUGIN_NAME = "MyCustomRolesMod (Paranoid Edition)";
        public const string PLUGIN_VERSION = "4.0.0";
    }
}
