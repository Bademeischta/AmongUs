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
        Jester,
        Echo,
        Geist
    }

    public class JesterRole : BaseRole
    {
        public override string Name => "Jester";
        public override Color Color => Color.magenta;
        public override RoleType RoleType => RoleType.Jester;

        public JesterRole(PlayerControl player) : base(player) { }
    }

    public class EchoRole : BaseRole
    {
        public override string Name => "Echo";
        public override Color Color => new Color(0.5f, 0.5f, 1f); // A light blue/purple
        public override RoleType RoleType => RoleType.Echo;

        public EchoRole(PlayerControl player) : base(player) { }
    }

    public class GeistRole : BaseRole
    {
        public override string Name => "Geist";
        public override Color Color => new Color(0.8f, 0.8f, 0.8f); // A light grey/white
        public override RoleType RoleType => RoleType.Geist;

        public GeistRole(PlayerControl player) : base(player) { }
    }
}
