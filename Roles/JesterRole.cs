using UnityEngine;

namespace MyCustomRolesMod.Roles
{
    public class JesterRole : BaseRole
    {
        public override string Name => "Jester";
        public override Color Color => Color.magenta;
        public override string Description => "You win if you get ejected!";

        public JesterRole(PlayerControl player) : base(player)
        {
        }

        public override void OnRoleAssign()
        {
            base.OnRoleAssign();
            // Logic specific to the Jester when the role is assigned
        }

        public override void Update()
        {
            base.Update();
            // Jester-specific per-frame logic, if any
        }
    }
}
