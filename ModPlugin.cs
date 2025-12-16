using BepInEx;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace MyCustomRolesMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class ModPlugin : BasePlugin
    {
        public static ManualLogSource Logger { get; private set; }
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public override void Load()
        {
            Logger = Log;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");

            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Logger.LogInfo($"Harmony patches applied for {PluginInfo.PLUGIN_NAME}.");
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.example.mycustomrolesmod.production";
        public const string PLUGIN_NAME = "MyCustomRolesMod (Production)";
        public const string PLUGIN_VERSION = "2.0.0";
    }
}
