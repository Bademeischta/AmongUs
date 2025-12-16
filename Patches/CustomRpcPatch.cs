using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using System.Collections.Generic;
using UnityEngine;
using static MyCustomRolesMod.ModPlugin;

namespace MyCustomRolesMod.Patches
{
    public enum CustomRpc : byte
    {
        PuppeteerControl = 220,
    }

    [HarmonyPatch]
    public static class CustomRpcPatch
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        [HarmonyPostfix]
        public static void HandleRpcPostfix(PlayerControl __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
        {
            switch ((CustomRpc)callId)
            {
                case CustomRpc.PuppeteerControl:
                    var targetId = reader.ReadByte();
                    var position = reader.ReadVector2();

                    if (PlayerControl.LocalPlayer.PlayerId == targetId)
                    {
                        PlayerControl.LocalPlayer.NetTransform.CmdSnapTo(position);
                    }
                    break;
            }
        }
    }
}
