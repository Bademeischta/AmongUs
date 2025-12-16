using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Reflection;

namespace MyCustomRolesMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MyCustomRolesMod : BasePlugin
    {
        public static MyCustomRolesMod Instance { get; private set; }
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public override void Load()
        {
            Instance = this;

            // Log that the plugin has started
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");

            // Apply all patches using Harmony
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.LogInfo($"Harmony patches applied for {PluginInfo.PLUGIN_NAME}.");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.example.mycustomrolesmod";
        public const string PLUGIN_NAME = "MyCustomRolesMod";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
