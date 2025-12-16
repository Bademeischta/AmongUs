using UnityEngine;

namespace MyCustomRolesMod.Roles
{
    public abstract class BaseRole
    {
        public abstract string Name { get; }
        public abstract Color Color { get; }
        public abstract string Description { get; }

        public PlayerControl Player { get; }

        protected BaseRole(PlayerControl player)
        {
            Player = player;
        }

        public virtual void OnRoleAssign() { }
        public virtual void OnRoleClear() { }
        public virtual void Update() { }
    }
}
