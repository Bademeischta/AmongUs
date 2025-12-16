using System.Collections.Generic;
using MyCustomRolesMod.Core;
using MyCustomRolesMod.Networking.Packets;

namespace MyCustomRolesMod.Networking.Packets
{
    public class SyncAllRolesPacket : BasePacket
    {
        public override RpcType RpcType => RpcType.SyncAllRoles;
        public Dictionary<byte, RoleType> Roles { get; set; }
    }
}
