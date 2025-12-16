namespace MyCustomRolesMod.Networking.Packets
{
    public class AckPacket : BasePacket
    {
        public override RpcType RpcType => RpcType.Acknowledge;
    }
}
