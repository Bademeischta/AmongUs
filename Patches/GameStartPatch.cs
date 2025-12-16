using HarmonyLib;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking;
using MyCustomRolesMod.Networking.Packets;
using System.Linq;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.Begin))]
    public static class GameStartPatch
    {
        private static readonly System.Random _random = new System.Random();

        public static void Postfix()
        {
            if (!AmongUsClient.Instance.AmHost) return;

            ModPlugin.Logger.LogInfo("[GameStart] Host is assigning roles...");
            RoleManager.Instance.ClearAllRoles();

            if (_random.Next(0, 100) < ModPlugin.ModConfig.JesterChance.Value)
            {
                var crewmates = PlayerControl.AllPlayerControls
                    .ToArray()
                    .Where(p => !p.Data.IsDead && p.Data.Role.Role == RoleTypes.Crewmate)
                    .ToList();

                if (crewmates.Any())
                {
                    var jester = crewmates[_random.Next(crewmates.Count)];

                    var writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SetRole);
                    writer.Write(jester.PlayerId);
                    writer.Write((byte)RoleType.Jester);
                    writer.EndMessage();

                    RpcManager.Instance.Send(writer);
                    RoleManager.Instance.SetRole(jester, RoleType.Jester); // Set locally after send
                }
            }
        }
    }
}
