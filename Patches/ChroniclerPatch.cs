using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch]
    public static class ChroniclerPatch
    {
        private static void SendFactToServer(string fact)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                ChroniclerManager.Instance.AddFact(fact);
            }
            else
            {
                var writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte)RpcType.CmdRecordFact);
                writer.Write(fact);
                writer.EndMessage();
                AmongUsClient.Instance.SendOrDisconnect(writer);
                writer.Recycle();
            }
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
        [HarmonyPostfix]
        public static void OnVentUse(Vent __instance)
        {
            var player = __instance.Enterer;
            if (player != null)
            {
                SendFactToServer($"{player.Data.PlayerName} used a vent.");
            }
        }

        [HarmonyPatch(typeof(PlayerTask), nameof(PlayerTask.Complete))]
        [HarmonyPostfix]
        public static void OnTaskComplete(PlayerTask __instance)
        {
            var player = __instance.Owner;
            if (player != null)
            {
                SendFactToServer($"{player.Data.PlayerName} completed a task.");
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
        [HarmonyPostfix]
        public static void OnPlayerDie(PlayerControl __instance, DeathReason reason)
        {
            SendFactToServer($"{__instance.Data.PlayerName} died.");
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
        [HarmonyPostfix]
        public static void OnRoundEnd()
        {
            ChroniclerManager.Instance.ClearFacts();
        }
    }
}