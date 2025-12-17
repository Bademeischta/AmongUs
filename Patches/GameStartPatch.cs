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

            var availableCrewmates = allPlayers.Where(p => p.Data.Role.Role == RoleTypes.Crewmate).ToList();
            var availableImpostors = allPlayers.Where(p => p.Data.Role.Role == RoleTypes.Impostor).ToList();
            var originalImpostors = new List<PlayerControl>(availableImpostors);

            // --- Geist Assignment (Impostor Role) ---
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

            // --- Balance Check ---
            if (availableImpostors.Count == 0 && originalImpostors.Any())
            {
                ModPlugin.Logger.LogWarning("[GameStart] All impostors became custom roles! Reverting one...");

                var playerToRevert = RoleManager.Instance.GetAllRoles()
                    .Where(kvp => kvp.Value.RoleType == RoleType.Geist)
                    .Select(kvp => GameData.Instance.GetPlayerById(kvp.Key)?.Object)
                    .FirstOrDefault(p => p != null && originalImpostors.Contains(p));

                if (playerToRevert != null)
                {
                    RoleManager.Instance.ClearRole(playerToRevert.PlayerId);

                    var writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SetRole);
                    writer.Write(playerToRevert.PlayerId);
                    writer.Write((byte)RoleType.None); // Assuming RoleType.None will clear the role
                    writer.EndMessage();
                    RpcManager.Instance.Send(writer);
                }
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
