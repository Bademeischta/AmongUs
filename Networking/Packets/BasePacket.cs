namespace MyCustomRolesMod.Networking.Packets
{
    public enum RpcType : byte
    {
        // Host -> Client
        SetRole,
        SyncAllRoles,
        SyncOptions,

        // Client -> Host
        Acknowledge,
    }

    public abstract class BasePacket
    {
        public abstract RpcType RpcType { get; }
        public uint MessageId { get; set; }
    }
}
