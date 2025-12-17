namespace MyCustomRolesMod.Networking.Packets
{
    public enum RpcType : byte
    {
        // Handshake
        VersionCheck,
        VersionResponse,
        Disconnect,

        // Gameplay
        SetRole,
        SyncAllRoles,
        SyncOptions,

        // Echo Role
        SetInfectedWord, // Client -> Host
        SyncInfectedWord, // Host -> Client
        CmdPlayerUsedInfectedWord, // Client -> Host
        RpcPlayerUsedInfectedWord, // Host -> Client

        // Geist Role
        MarkPlayer, // Client -> Host
        SyncMarkedPlayer, // Host -> Client
        SetFakeTimeOfDeath, // Host -> Client

        // Reliability
        Acknowledge
    }

    public abstract class BasePacket
    {
        public uint SequenceId { get; set; }
    }

    // Host -> Client
    public class VersionCheckPacket : BasePacket
    {
        public byte ProtocolVersion { get; set; }
    }

    // Client -> Host
    public class VersionResponsePacket : BasePacket
    {
        public byte ProtocolVersion { get; set; }
    }

    // Host -> Client
    public class DisconnectPacket : BasePacket
    {
        public byte Reason { get; set; } // Could be an enum later
    }
}
