using MyCustomRolesMod.Core;

namespace MyCustomRolesMod.Networking.Packets
{
    public class RolePacket : BasePacket
    {
        public byte PlayerId { get; set; }
        public RoleType Role { get; set; }
    }
}
