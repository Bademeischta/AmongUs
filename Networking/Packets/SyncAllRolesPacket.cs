using System.Collections.Generic;
using MyCustomRolesMod.Core;

namespace MyCustomRolesMod.Networking.Packets
{
    public class SyncAllRolesPacket : BasePacket
    {
        public Dictionary<byte, RoleType> Roles { get; set; }
    }
}
