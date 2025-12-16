using BepInEx.Configuration;

namespace MyCustomRolesMod.Config
{
    public class ModConfig
    {
        public ConfigEntry<float> JesterChance { get; }
        public ConfigEntry<bool> IsDebug { get; }
        public ConfigEntry<float> RpcTimeoutSeconds { get; }
        public ConfigEntry<int> MaxRpcRetries { get; }

        public ModConfig(ConfigFile config)
        {
            JesterChance = config.Bind(
                "Gameplay",
                "JesterChance",
                100f,
                "Probability of a Jester spawning in a game (0-100%)."
            );

            IsDebug = config.Bind(
                "Debug",
                "EnableDebugLogging",
                false,
                "Enables detailed logging for debugging purposes."
            );

            RpcTimeoutSeconds = config.Bind(
                "Network",
                "RpcTimeoutSeconds",
                2.0f,
                "Time in seconds before a network message is considered timed out."
            );

            MaxRpcRetries = config.Bind(
                "Network",
                "MaxRpcRetries",
                3,
                "Maximum number of times to resend a message that has not been acknowledged."
            );
        }
    }
}
