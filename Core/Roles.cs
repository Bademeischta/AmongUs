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
        Geist,
        Witness,
        Puppeteer,
        Glitch,
        Auditor,
        Phantom
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

    public class WitnessRole : BaseRole
    {
        public override string Name => "Witness";
        public override Color Color => new Color(0f, 0.8f, 0.8f); // A bright cyan
        public override RoleType RoleType => RoleType.Witness;

        public WitnessRole(PlayerControl player) : base(player) { }
    }

    public class PuppeteerRole : BaseRole
    {
        public override string Name => "Puppeteer";
        public override Color Color => new Color(0.5f, 0f, 0.5f); // A deep purple
        public override RoleType RoleType => RoleType.Puppeteer;

        public PuppeteerRole(PlayerControl player) : base(player) { }
    }

    public class GlitchRole : BaseRole
    {
        public override string Name => "Glitch";
        public override Color Color => new Color(0f, 1f, 0f); // A lime green
        public override RoleType RoleType => RoleType.Glitch;



        public GlitchRole(PlayerControl player) : base(player) { }
    }

    public class AuditorRole : BaseRole
    {
        public override string Name => "Auditor";
        public override Color Color => new Color(1f, 0.84f, 0f); // Gold
        public override RoleType RoleType => RoleType.Auditor;

        public AuditorRole(PlayerControl player) : base(player) { }
    }

    public class PhantomRole : BaseRole
    {
        public override string Name => "Phantom";
        public override Color Color => new Color(0.5f, 0f, 1f); // Purple
        public override RoleType RoleType => RoleType.Phantom;

        public PhantomRole(PlayerControl player) : base(player) { }
    }
}
