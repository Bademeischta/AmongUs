using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class KillButtonPatch
    {
        public static bool Prefix(KillButton __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);

            if (role?.RoleType != RoleType.Geist)
            {
                return true; // Perform the original kill logic
            }

            var target = __instance.currentTarget;
            if (target != null && !target.Data.IsDead)
            {
                // Send an RPC to the host to mark the player
                var writer = Hazel.MessageWriter.Get(Hazel.SendOption.Reliable);
                writer.StartMessage((byte)RpcType.MarkPlayer);
                writer.Write(target.PlayerId);
                writer.EndMessage();
                RpcManager.Instance.Send(writer);

                // Manually set the kill cooldown
                localPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
            }

            return false; // Prevent the original kill logic from running
        }
    }
}
