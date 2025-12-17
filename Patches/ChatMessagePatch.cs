using HarmonyLib;
using MyCustomRolesMod.Core;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class ChatMessagePatch
    {
        public static bool Prefix(ChatController __instance)
        {
            if (!MeetingHud.Instance) return true;

            var localPlayer = PlayerControl.LocalPlayer;
            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            var text = __instance.TextArea.text;

            if (string.IsNullOrWhiteSpace(text)) return true;

            // Check for infected word BEFORE anything else
            var infectedWord = EchoManager.Instance.InfectedWord;
            if (!string.IsNullOrWhiteSpace(infectedWord) && text.ToLower().Contains(infectedWord))
            {
                var writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte)RpcType.CmdPlayerUsedInfectedWord);
                writer.Write(localPlayer.PlayerId);
                writer.EndMessage();
                AmongUsClient.Instance.SendOrDisconnect(writer);
            }

            // Check for /infect command
            if (role?.RoleType == RoleType.Echo && text.StartsWith("/infect "))
            {
                var word = text.Substring(8).Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(word))
                {
                    var writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SetInfectedWord);
                    writer.Write(word);
                    writer.EndMessage();
                    AmongUsClient.Instance.SendOrDisconnect(writer);
                }
                __instance.TextArea.Clear();
                return false; // Block original method
            }

            return true; // Allow normal chat
        }
    }
}
