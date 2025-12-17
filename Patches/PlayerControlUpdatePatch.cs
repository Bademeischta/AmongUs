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
            // Only run host logic once per frame on a single object.
            if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId) return;

            var geistPlayer = GeistManager.Instance.GeistPlayer;
            if (geistPlayer == null) return;

            GeistManager.Instance.UpdateMarkedPlayers(Time.time, (playerId) =>
            {
                var player = GameData.Instance.GetPlayerById(playerId)?.Object;
                if (player != null && !player.Data.IsDead)
                {
                    geistPlayer.RpcMurderPlayer(player, true);
                }
            });
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcMurderPlayer))]
    public static class PlayerControlRpcMurderPlayerPatch
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl victim)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            byte victimId = victim.PlayerId;
            if (GeistManager.Instance.IsMarked(victimId))
            {
                float fakeTime = GeistManager.Instance.GetTimeOfDeath(victimId);

                DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == victimId);

                if (body != null)
                {
                    body.TimeOfDeath = fakeTime;

                    var writer = Hazel.MessageWriter.Get(Hazel.SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SetFakeTimeOfDeath);
                    writer.Write(body.NetId);
                    writer.Write(fakeTime);
                    writer.EndMessage();
                    RpcManager.Instance.Send(writer);
                }

                // This is now handled in UpdateMarkedPlayers
                // GeistManager.Instance.RemoveMark(victimId);
            }
        }
    }
}
