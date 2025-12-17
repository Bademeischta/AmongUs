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
            var initialCrewmates = allPlayers.Where(p => p.Data.Role.Role == RoleTypes.Crewmate).ToList();
            var initialImpostors = allPlayers.Where(p => p.Data.Role.Role == RoleTypes.Impostor).ToList();

            var availableCrewmates = new List<PlayerControl>(initialCrewmates);
            var availableImpostors = new List<PlayerControl>(initialImpostors);

            // --- Geist Assignment (Impostor Role) ---
            // Ensure at least one vanilla impostor remains, if possible.
            if (availableImpostors.Count > 1 && _random.Next(0, 100) < ModPlugin.ModConfig.GeistChance.Value)
            {
                var geist = availableImpostors[_random.Next(availableImpostors.Count)];
                AssignRole(geist, RoleType.Geist);
                availableImpostors.Remove(geist);
            }

            // --- Jester Assignment (Crewmate Role) ---
            if (availableCrewmates.Any() && _random.Next(0, 100) < ModPlugin.ModConfig.JesterChance.Value)
            {
                var jester = availableCrewmates[_random.Next(availableCrewmates.Count)];
                AssignRole(jester, RoleType.Jester);
                availableCrewmates.Remove(jester);
            }

            // --- Echo Assignment (Crewmate Role) ---
            if (availableCrewmates.Any() && _random.Next(0, 100) < ModPlugin.ModConfig.EchoChance.Value)
            {
                var echo = availableCrewmates[_random.Next(availableCrewmates.Count)];
                AssignRole(echo, RoleType.Echo);
                availableCrewmates.Remove(echo);
            }

            // --- Balance Validation ---
            if (availableImpostors.Count == 0 && initialImpostors.Any())
            {
                ModPlugin.Logger.LogWarning("[GameStart] All impostors were converted to custom roles. This may unbalance the game.");
                // In a more advanced implementation, we might revert a role assignment here.
            }
        }

        private static void AssignRole(PlayerControl player, RoleType roleType)
        {
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
