using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using System.Linq;
using Hazel;
using System.Collections.Generic;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.Begin))]
    public static class GameStartPatch
    {
        private static readonly System.Random _random = new System.Random();

        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost) return;

            RoleManager.Instance.ClearAllRoles();
            EchoManager.Instance.Clear();
            GeistManager.Instance.Clear();

            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(p => !p.Data.IsDead).ToList();
            var crewmates = allPlayers.Where(p => p.Data.Role.Role == RoleTypes.Crewmate).ToList();
            var impostors = allPlayers.Where(p => p.Data.Role.Role == RoleTypes.Impostor).ToList();

            // --- Geist Assignment (Impostor Role) ---
            if (impostors.Any() && _random.Next(0, 100) < ModPlugin.ModConfig.GeistChance.Value)
            {
                var geist = impostors[_random.Next(impostors.Count)];
                AssignRole(geist, RoleType.Geist);
                impostors.Remove(geist);
            }

            // --- Jester Assignment (Crewmate Role) ---
            if (crewmates.Any() && _random.Next(0, 100) < ModPlugin.ModConfig.JesterChance.Value)
            {
                var jester = crewmates[_random.Next(crewmates.Count)];
                AssignRole(jester, RoleType.Jester);
                crewmates.Remove(jester);
            }

            // --- Echo Assignment (Crewmate Role) ---
            if (crewmates.Any() && _random.Next(0, 100) < ModPlugin.ModConfig.EchoChance.Value)
            {
                var echo = crewmates[_random.Next(crewmates.Count)];
                AssignRole(echo, RoleType.Echo);
                crewmates.Remove(echo);
            }
        }

        private static void AssignRole(PlayerControl player, RoleType roleType)
        {
            // CRITICAL: Set state LOCALLY first.
            RoleManager.Instance.SetRole(player, roleType);

            if (roleType == RoleType.Geist)
            {
                GeistManager.Instance.GeistPlayer = player;
            }

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SetRole);
            writer.Write(player.PlayerId);
            writer.Write((byte)roleType);
            writer.EndMessage();

            RpcManager.Instance.Send(writer);
        }
    }
}
