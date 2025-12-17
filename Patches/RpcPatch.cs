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
            var originalPosition = reader.Position;
            try
            {
                var rpcType = (RpcType)reader.Tag;

                if (Enum.IsDefined(typeof(RpcType), rpcType))
                {
                    try
                    {
                        RpcManager.Instance.HandleMessage(rpcType, reader, senderId);
                    }
                    catch (Exception e)
                    {
                        ModPlugin.Logger.LogError($"[RPC Error] Failed to handle mod message: {e}");
                    }
                }
            }
            finally
            {
                reader.Position = originalPosition;
            }
        }
    }
}
