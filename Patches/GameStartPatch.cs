using HarmonyLib;
using MyCustomRolesMod.Management;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.Begin))]
    public static class GameStartPatch
    {
        public static void Postfix()
        {
            // The host is responsible for assigning roles
            if (!PlayerControl.LocalPlayer.AmOwner) return;

            RoleManager.ClearRoles();

            var players = PlayerControl.AllPlayerControls;
            if (players.Count <= 0) return;

            var jesterChance = CustomGameOptions.JesterChance;
            if (new System.Random().Next(0, 100) < jesterChance)
            {
                var potentialJesters = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
                foreach (var player in players)
                {
                    if (player.Data.Role.Role == RoleTypes.Crewmate)
                    {
                        potentialJesters.Add(player);
                    }
                }

                if (potentialJesters.Count > 0)
                {
                    var jester = potentialJesters[new System.Random().Next(0, potentialJesters.Count)];
                    RoleManager.SetRole(jester, RoleType.Jester);
                    NetworkManager.SendRoleAssignment(jester, RoleType.Jester);
                }
            }
        }
    }
}
