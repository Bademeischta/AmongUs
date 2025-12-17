using HarmonyLib;
using MyCustomRolesMod.Core;
using Hazel;

namespace MyCustomRolesMod.Patches
{
    [HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
    public static class ChatMessagePatch
    {
        // Change return type to bool to allow cancelling the original method.
        public static bool Prefix(ChatController __instance)
        {
            if (!MeetingHud.Instance) return true; // Only allow infection during meetings

            var localPlayer = PlayerControl.LocalPlayer;
            var role = RoleManager.Instance.GetRole(localPlayer.PlayerId);
            if (role?.RoleType != RoleType.Echo) return true;

            var text = __instance.TextArea.text;
            if (string.IsNullOrWhiteSpace(text)) return true;

            if (text.StartsWith("/infect "))
            {
                var word = text.Substring(8).Trim().ToLower();
                if (!string.IsNullOrWhiteSpace(word))
                {
                    // As a client, send the selected word to the host.
                    var writer = MessageWriter.Get(SendOption.Reliable);
                    writer.StartMessage((byte)RpcType.SetInfectedWord);
                    writer.Write(word);
                    writer.EndMessage();
                    AmongUsClient.Instance.SendOrDisconnect(writer);
                }

                // Clear the text area and prevent the original method from running.
                __instance.TextArea.Clear();
                return false;
            }

            return true; // Continue with original method for normal chat messages.
        }

        public static void Postfix(ChatController __instance)
        {
            var text = __instance.TextArea.text;
            var infectedWord = EchoManager.Instance.InfectedWord;
            var localPlayer = PlayerControl.LocalPlayer;

            if (!string.IsNullOrWhiteSpace(infectedWord) && text.ToLower().Contains(infectedWord))
            {
                var writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte)RpcType.CmdPlayerUsedInfectedWord);
                writer.Write(localPlayer.PlayerId);
                writer.EndMessage();
                AmongUsClient.Instance.SendOrDisconnect(writer);
            }
        }
    }
}
