using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using UnityEngine;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class PlayerControlUpdatePatch
    {
        private static int _dendroTick;

        public static void Postfix(PlayerControl __instance)
        {
            // Host handles the authoritative tracking for Dendrochronologist
            if (AmongUsClient.Instance.AmHost && __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                if (_dendroTick++ % 10 == 0)
                {
                    DendrochronologistManager.Instance.UpdateTracking();
                }
            }

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
            if (!AmongUsClient.Instance.AmHost) return;

            byte victimId = victim.PlayerId;

            // --- Solipsist Passive: Weightless Crime ---
            var role = RoleManager.Instance.GetRole(__instance.PlayerId);
            if (role?.RoleType == RoleType.Solipsist)
            {
                var victimPos = victim.GetTruePosition();
                var nearestCrew = PlayerControl.AllPlayerControls
                    .Where(p => p.PlayerId != __instance.PlayerId && p.PlayerId != victimId && !p.Data.IsDead)
                    .OrderBy(p => Vector2.Distance(victimPos, p.GetTruePosition()))
                    .FirstOrDefault();

                if (nearestCrew != null && Vector2.Distance(victimPos, nearestCrew.GetTruePosition()) < 5f)
                {
                    DeadBody body = Object.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == victimId);
                    if (body != null)
                    {
                        RpcManager.Instance.SendSetCensoredObject(nearestCrew.PlayerId, body.NetId);
                    }
                }
            }

            // --- Geist Logic ---
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