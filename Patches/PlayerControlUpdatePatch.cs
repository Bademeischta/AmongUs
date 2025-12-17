using HarmonyLib;
using MyCustomRolesMod.Core;
using UnityEngine;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlUpdatePatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId) return;

            var geist = GeistManager.Instance.GeistPlayer;
            if (geist == null) return;

            GeistManager.Instance.UpdateMarkedPlayers(Time.time, (playerId) =>
            {
                var player = GameData.Instance.GetPlayerById(playerId)?.Object;
                if (player != null && !player.Data.IsDead)
                {
                    geist.RpcMurderPlayer(player, true);
                }
            });
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcMurderPlayer))]
    public static class PlayerControlRpcMurderPlayerPatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl victim)
        {
            // The host has the authoritative state on who is marked.
            if (!AmongUsClient.Instance.AmHost) return;

            byte victimId = victim.PlayerId;
            if (GeistManager.Instance.IsMarked(victimId))
            {
                float fakeTime = GeistManager.Instance.GetTimeOfDeath(victimId);

                // Find the dead body that was just created.
                DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == victimId);

                if (body != null)
                {
                    // The host sets it locally...
                    body.TimeOfDeath = fakeTime;

                    // ...and then tells the clients to do the same.
                    var writer = Hazel.MessageWriter.Get(Hazel.SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SetFakeTimeOfDeath);
                    writer.Write(body.NetId);
                    writer.Write(fakeTime);
                    writer.EndMessage();
                    RpcManager.Instance.Send(writer);
                }

                // The player has now been killed, so we can remove the mark.
                GeistManager.Instance.RemoveMark(victimId);
            }
        }
    }
}
