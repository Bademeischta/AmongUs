using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public abstract class BaseRole
    {
        public PlayerControl Player { get; }
        public abstract string Name { get; }
        public abstract Color Color { get; }
        public abstract RoleType RoleType { get; }

        protected BaseRole(PlayerControl player)
        {
            Player = player;
        }
    }

    public enum RoleType : byte
    {
        None,
        Jester
    }

    public class JesterRole : BaseRole
    {
        public override string Name => "Jester";
        public override Color Color => Color.magenta;
        public override RoleType RoleType => RoleType.Jester;

        public JesterRole(PlayerControl player) : base(player) { }
    }
}
