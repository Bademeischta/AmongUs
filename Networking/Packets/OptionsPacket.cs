using Hazel;
using System;

namespace MyCustomRolesMod.Networking.Packets
{
    public class OptionsPacket : BasePacket
    {
        public override RpcType RpcType => RpcType.SyncOptions;

        private const byte ProtocolVersion = 1;
        private const uint MagicBytes = 0xDEADBEEF;

        public float JesterChance { get; set; }

        public void Serialize(MessageWriter writer)
        {
            writer.Write(MagicBytes);
            writer.Write(ProtocolVersion);
            writer.Write(JesterChance);
            writer.Write(CalculateChecksum());
        }

        public static OptionsPacket Deserialize(MessageReader reader)
        {
            if (reader.ReadUInt32() != MagicBytes) throw new Exception("Invalid magic bytes");
            if (reader.ReadByte() > ProtocolVersion) throw new Exception("Unsupported protocol version");

            var packet = new OptionsPacket
            {
                JesterChance = reader.ReadSingle()
            };

            var checksum = reader.ReadUInt32();
            if (checksum != packet.CalculateChecksum()) throw new Exception("Checksum mismatch");

            return packet;
        }

        private uint CalculateChecksum()
        {
            // Simple checksum for demonstration. A real implementation would use CRC32.
            return (uint)(JesterChance * 42);
        }
    }
}
