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
            uint crc = 0xFFFFFFFF;
            var bytes = BitConverter.GetBytes(JesterChance);
            foreach (var b in bytes)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    crc = (crc >> 1) ^ (0xEDB88320 & ~((crc & 1) - 1));
                }
            }
            return ~crc;
        }
    }
}
