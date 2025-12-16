using HarmonyLib;
using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using System.Linq;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.Begin))]
    public static class GameStartPatch
    {
        private static readonly System.Random random = new System.Random();

        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                ModPlugin.Logger.LogInfo("Not the host, skipping role assignment.");
                return;
            }

            ModPlugin.Logger.LogInfo("Host is assigning roles...");
            RoleManager.Instance.ClearAllRoles();

            float jesterChance = CustomGameOptions.JesterChance;

            if (random.Next(0, 100) < jesterChance)
            {
                var crewmates = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(p => !p.Data.IsDead && p.Data.Role.Role == RoleTypes.Crewmate)
                    .ToList();

                if (crewmates.Any())
                {
                    var jester = crewmates[random.Next(crewmates.Count)];
                    RoleManager.Instance.SetRole(jester, RoleType.Jester);
                    NetworkManager.SendRoleAssignment(jester, RoleType.Jester);
                }
                else
                {
                    ModPlugin.Logger.LogWarning("No potential crewmates found to become Jester.");
                }
            }
        }
    }
}
