using System.Collections.Generic;
using Hazel;
using MyCustomRolesMod.Networking.Packets;
using UnityEngine;
using MyCustomRolesMod.Core;
using System;
using MyCustomRolesMod.Patches;

namespace MyCustomRolesMod.Networking
{
    public class RpcManager : MonoBehaviour
    {
        public static RpcManager Instance { get; private set; }

        private readonly Dictionary<uint, PendingRpc> _pendingRpcs = new Dictionary<uint, PendingRpc>();
        private readonly List<uint> _toRemove = new List<uint>();
        private uint _nextMessageId = 0;
        private readonly Queue<uint> _receivedMessageIds = new Queue<uint>();

        void Awake() => Instance = this;

        void OnDestroy()
        {
            foreach (var rpc in _pendingRpcs.Values) rpc.Writer.Recycle();
            _pendingRpcs.Clear();
        }

        void Update()
        {
            if (AmongUsClient.Instance.AmHost) HandleTimeouts();
        }

        private void HandleTimeouts()
        {
            var now = Time.time;
            _toRemove.Clear();

            foreach (var kvp in _pendingRpcs)
            {
                var rpc = kvp.Value;
                if (now > rpc.NextSendAttemptTime)
                {
                    if (rpc.RetryCount < ModPlugin.ModConfig.MaxRpcRetries.Value)
                    {
                        rpc.RetryCount++;
                        rpc.NextSendAttemptTime = now + Mathf.Pow(2, rpc.RetryCount);
                        ModPlugin.Logger.LogWarning($"[RPC] Retrying message {rpc.MessageId}...");
                        AmongUsClient.Instance.SendOrDisconnect(rpc.Writer, rpc.TargetClientId ?? -1);
                    }
                    else
                    {
                        _toRemove.Add(kvp.Key);
                    }
                }
            }

            foreach (var key in _toRemove)
            {
                if (_pendingRpcs.TryGetValue(key, out var rpc))
                {
                    _pendingRpcs.Remove(key);
                    rpc.Writer.Recycle();
                }
            }
        }

        public void Send(MessageWriter writer) => SendTo(writer, null);

        public void SendTo(MessageWriter writer, int? targetClientId)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                writer.Recycle();
                return;
            }

            var messageId = _nextMessageId++;
            var finalWriter = MessageWriter.Get(SendOption.Reliable);
            finalWriter.StartMessage((byte)writer.Tag);
            finalWriter.Write(messageId);
            finalWriter.WriteBytesAndSize(writer.Buffer, writer.Length);
            finalWriter.EndMessage();

            writer.Recycle();

            _pendingRpcs[messageId] = new PendingRpc { MessageId = messageId, Timestamp = Time.time, NextSendAttemptTime = Time.time + ModPlugin.ModConfig.RpcTimeoutSeconds.Value, Writer = finalWriter, TargetClientId = targetClientId };
            AmongUsClient.Instance.SendOrDisconnect(finalWriter, targetClientId ?? -1);
        }

        public void HandleMessage(RpcType rpcType, MessageReader reader, int senderId)
        {
            MessageReader payloadReader = null;
            try
            {
                if (rpcType == RpcType.Acknowledge) { HandleAck(reader.ReadUInt32()); return; }
                if (rpcType == RpcType.VersionCheck) { HandleVersionCheck(reader); return; }
                if (rpcType == RpcType.VersionResponse) { HandleVersionResponse(reader, senderId); return; }

                var messageId = reader.ReadUInt32();
                if (!AmongUsClient.Instance.AmHost && HasBeenReceived(messageId)) { SendAck(messageId); return; }

                var payload = reader.ReadBytesAndSize();
                payloadReader = MessageReader.Get(payload);

                switch (rpcType)
                {
                    case RpcType.SetRole: HandleSetRole(payloadReader); break;
                    case RpcType.SyncAllRoles: HandleSyncAllRoles(payloadReader); break;
                    case RpcType.SyncOptions: HandleSyncOptions(payloadReader); break;
                    default: ModPlugin.Logger.LogWarning($"[RPC] Unhandled message type: {rpcType}"); break;
                }

                if (!AmongUsClient.Instance.AmHost) SendAck(messageId);
            }
            finally
            {
                payloadReader?.Recycle();
            }
        }

        private void HandleAck(uint messageId)
        {
            if (_pendingRpcs.TryGetValue(messageId, out var rpc))
            {
                _pendingRpcs.Remove(messageId);
                rpc.Writer.Recycle();
            }
        }

        private void SendAck(uint messageId)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.Acknowledge);
            writer.Write(messageId);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer);
            writer.Recycle();
        }

        private bool HasBeenReceived(uint messageId)
        {
            if (_receivedMessageIds.Contains(messageId)) return true;
            _receivedMessageIds.Enqueue(messageId);
            if (_receivedMessageIds.Count > 100) _receivedMessageIds.Dequeue();
            return false;
        }

        private void HandleVersionCheck(MessageReader reader)
        {
            byte hostVersion = reader.ReadByte();
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.VersionResponse);
            writer.Write(HandshakeManager.ProtocolVersion);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer);
            writer.Recycle();
        }

        private void HandleVersionResponse(MessageReader reader, int senderId)
        {
            byte clientVersion = reader.ReadByte();
            if (clientVersion != HandshakeManager.ProtocolVersion)
            {
                var writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte)RpcType.Disconnect);
                writer.EndMessage();
                AmongUsClient.Instance.SendOrDisconnect(writer, senderId);
                writer.Recycle();
            }
            else
            {
                 HandshakeManager.UnverifiedClients.Remove(senderId);
            }
        }

        private void HandleSetRole(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var roleType = (RoleType)reader.ReadByte();
            if (!Enum.IsDefined(typeof(RoleType), roleType)) return;
            var player = GameData.Instance.GetPlayerById(playerId)?.Object;
            if (player != null) RoleManager.Instance.SetRole(player, roleType);
        }

        private void HandleSyncAllRoles(MessageReader reader)
        {
            var count = reader.ReadUInt16();
            for (int i = 0; i < count; i++)
            {
                var playerId = reader.ReadByte();
                var roleType = (RoleType)reader.ReadByte();
                if (!Enum.IsDefined(typeof(RoleType), roleType)) continue;
                var player = GameData.Instance.GetPlayerById(playerId)?.Object;
                if (player != null) RoleManager.Instance.SetRole(player, roleType);
            }
        }

        private void HandleSyncOptions(MessageReader reader)
        {
            var packet = OptionsPacket.Deserialize(reader);
            ModPlugin.ModConfig.JesterChance.Value = packet.JesterChance;
        }

        private class PendingRpc
        {
            public uint MessageId;
            public float Timestamp;
            public float NextSendAttemptTime;
            public int RetryCount;
            public MessageWriter Writer;
            public int? TargetClientId;
        }
    }
}
