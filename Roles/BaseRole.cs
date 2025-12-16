using UnityEngine;

namespace MyCustomRolesMod.Roles
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

        public virtual void OnRoleAssign() { }
        public virtual void OnRoleClear() { }
    }

    public enum RoleType : byte
    {
        None,
        Jester
    }
}
