using MyCustomRolesMod.Core;

namespace MyCustomRolesMod.Networking.Packets
{
    public class RolePacket : BasePacket
    {
        public override RpcType RpcType => RpcType.SetRole;
        public byte PlayerId { get; set; }
        public RoleType Role { get; set; }
    }
}
