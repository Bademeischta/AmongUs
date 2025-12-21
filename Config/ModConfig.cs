using BepInEx.Configuration;

namespace MyCustomRolesMod.Config
{
    public class ModConfig
    {
        public ConfigEntry<float> JesterChance { get; }
        public ConfigEntry<float> EchoChance { get; }
        public ConfigEntry<float> GeistChance { get; }
        public ConfigEntry<float> ScribeChance { get; }
        public ConfigEntry<float> PhantomChance { get; }
        public ConfigEntry<bool> IsDebug { get; }
        public ConfigEntry<float> RpcTimeoutSeconds { get; }
        public ConfigEntry<int> MaxRpcRetries { get; }

        public ModConfig(ConfigFile config)
        {
            JesterChance = config.Bind("Gameplay", "JesterChance", 100f, "Probability of a Jester spawning (0-100%).");
            EchoChance = config.Bind("Gameplay", "EchoChance", 100f, "Probability of an Echo spawning (0-100%).");
            GeistChance = config.Bind("Gameplay", "GeistChance", 100f, "Probability of a Geist spawning (0-100%).");
            ScribeChance = config.Bind("Gameplay", "ScribeChance", 100f, "Probability of a Scribe spawning (0-100%).");
            PhantomChance = config.Bind("Gameplay", "PhantomChance", 100f, "Probability of a Phantom spawning (0-100%).");
            IsDebug = config.Bind("Debug", "EnableDebugLogging", false, "Enables verbose logging.");
            RpcTimeoutSeconds = config.Bind("Network", "RpcTimeoutSeconds", 2.0f, "Time before a message is considered timed out.");
            MaxRpcRetries = config.Bind("Network", "MaxRpcRetries", 3, "Max number of times to resend a message.");
        }
    }
}
