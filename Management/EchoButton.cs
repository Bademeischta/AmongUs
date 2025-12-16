using MyCustomRolesMod.Management;
using MyCustomRolesMod.Roles;
using UnityEngine;

namespace MyCustomRolesMod.Management
{
    public class EchoButton : MonoBehaviour
    {
        public void OnClick()
        {
            var echo = RoleManager.Instance.GetRole(PlayerControl.LocalPlayer.PlayerId) as EchoRole;
            if (echo == null || echo.RecordedChat == null) return;

            foreach (var message in echo.RecordedChat)
            {
                MeetingHud.Instance.AddChat(echo.RecordedPlayer, message);
            }

            echo.ClearRecording();
        }
    }
}
