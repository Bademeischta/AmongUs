using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using System;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HandleMessage))]
    public static class RpcPatch
    {
        public static void Postfix(InnerNetClient __instance, MessageReader reader, [HarmonyArgument(0)] int senderId)
        {
            var rpcType = (RpcType)reader.Tag;

            // Only process messages that are part of our mod's protocol.
            if (Enum.IsDefined(typeof(RpcType), rpcType))
            {
                // Save the original position of the reader's stream.
                int originalPosition = reader.Position;
                try
                {
                    RpcManager.Instance.HandleMessage(rpcType, reader, senderId);
                }
                catch (Exception e)
                {
                    ModPlugin.Logger.LogError($"[RPC Error] Failed to handle mod message: {e}");
                }
                finally
                {
                    // CRITICAL: Restore the reader's position so the game's original
                    // code can read the message from the beginning.
                    reader.Position = originalPosition;
                }
            }
        }
    }
}
