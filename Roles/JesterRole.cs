using UnityEngine;

namespace MyCustomRolesMod.Roles
{
    public class JesterRole : BaseRole
    {
        public override string Name => "Jester";
        public override Color Color => Color.magenta;
        public override RoleType RoleType => RoleType.Jester;

        public JesterRole(PlayerControl player) : base(player) { }
    }
}
