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
        Dendrochronologist,
        Solipsist
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

    public class DendrochronologistRole : BaseRole
    {
        public override string Name => "Dendrochronologist";
        public override Color Color => new Color(0.6f, 0.4f, 0.2f); // Brown
        public override RoleType RoleType => RoleType.Dendrochronologist;

        public DendrochronologistRole(PlayerControl player) : base(player) { }
    }

    public class SolipsistRole : BaseRole
    {
        public override string Name => "Solipsist";
        public override Color Color => new Color(1f, 0.5f, 0f); // Orange
        public override RoleType RoleType => RoleType.Solipsist;

        public SolipsistRole(PlayerControl player) : base(player) { }
    }
}
