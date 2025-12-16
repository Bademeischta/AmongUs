using HarmonyLib;
using Hazel;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using System;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.HandleMessage))]
    public static class RpcPatch
    {
        // Use Postfix to avoid interfering with the game's original message handling.
        public static void Postfix(InnerNetClient __instance, MessageReader reader, [HarmonyArgument(0)] int senderId)
        {
            // Create a copy of the reader to avoid messing with the original stream position.
            MessageReader readerCopy = reader.Copy();
            var rpcType = (RpcType)readerCopy.Tag;

            // Only process messages that are part of our mod's protocol.
            if (Enum.IsDefined(typeof(RpcType), rpcType))
            {
                try
                {
                    RpcManager.Instance.HandleMessage(rpcType, readerCopy, senderId);
                }
                catch (Exception e)
                {
                    ModPlugin.Logger.LogError($"[RPC Error] Failed to handle mod message: {e}");
                }
            }
        }
    }
}
