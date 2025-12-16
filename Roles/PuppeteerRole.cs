using UnityEngine;

namespace MyCustomRolesMod.Roles
{
    public class PuppeteerRole : BaseRole
    {
        public override string Name => "Puppeteer";
        public override Color Color => new Color(0.8f, 0.4f, 0.2f); // A burnt orange
        public override RoleType RoleType => RoleType.Puppeteer;
        public override TeamType Team => TeamType.Impostor;

        public PlayerControl Target { get; private set; }
        public bool IsControlling { get; private set; }
        public bool IsSelectingTarget { get; private set; }

        public PuppeteerRole(PlayerControl player) : base(player) { }

        public void SetTarget(PlayerControl target)
        {
            Target = target;
        }

        public void StartControl()
        {
            IsControlling = true;
        }

        public void StopControl()
        {
            IsControlling = false;
        }

        public void ClearTarget()
        {
            Target = null;
        }

        public void StartSelection()
        {
            IsSelectingTarget = true;
        }

        public void StopSelection()
        {
            IsSelectingTarget = false;
        }
    }
}
