using UnityEngine;

namespace MyCustomRolesMod.Roles
{
    public class EchoRole : BaseRole
    {
        public override string Name => "Echo";
        public override Color Color => new Color(0.5f, 0.5f, 1f); // A light blue/purple
        public override RoleType RoleType => RoleType.Echo;
        public override TeamType Team => TeamType.Crewmate;

        public PlayerControl RecordedPlayer { get; private set; }
        public string[] RecordedChat { get; private set; }

        public EchoRole(PlayerControl player) : base(player) { }

        public void Record(PlayerControl target, string[] chat)
        {
            RecordedPlayer = target;
            RecordedChat = chat;
        }

        public void ClearRecording()
        {
            RecordedPlayer = null;
            RecordedChat = null;
        }
    }
}
