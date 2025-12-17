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

        // Use a HashSet for efficient O(1) lookups
        private readonly HashSet<uint> _receivedMessageIds = new HashSet<uint>();
        private const int ReceivedMessageCacheSize = 100;

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
                // Clients send messages directly, they don't use the Send/SendTo from RpcManager
                AmongUsClient.Instance.SendOrDisconnect(writer);
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

                if (AmongUsClient.Instance.AmHost)
                {
                    // Host-side message handling
                    var messageId = reader.ReadUInt32();
                    if(HasBeenReceived(messageId)) return;

                    var payload = reader.ReadBytesAndSize();
                    payloadReader = MessageReader.Get(payload);

                    HandleHostMessage(rpcType, payloadReader, senderId);

                    SendAck(messageId, senderId);
                }
                else
                {
                    // Client-side message handling
                    var payload = reader.ReadBytesAndSize();
                    payloadReader = MessageReader.Get(payload);
                    HandleClientMessage(rpcType, payloadReader);
                }
            }
            finally
            {
                payloadReader?.Recycle();
            }
        }

        private void HandleHostMessage(RpcType rpcType, MessageReader reader, int senderId)
        {
            switch (rpcType)
            {
                case RpcType.SetInfectedWord: HandleSetInfectedWord(reader); break;
                case RpcType.CmdPlayerUsedInfectedWord: HandleCmdPlayerUsedInfectedWord(reader, senderId); break;
                case RpcType.MarkPlayer: HandleMarkPlayer(reader); break;
                default: ModPlugin.Logger.LogWarning($"[RPC] Unhandled host message type: {rpcType}"); break;
            }
        }

        private void HandleClientMessage(RpcType rpcType, MessageReader reader)
        {
            switch (rpcType)
            {
                case RpcType.SetRole: HandleSetRole(reader); break;
                case RpcType.SyncAllRoles: HandleSyncAllRoles(reader); break;
                case RpcType.SyncOptions: HandleSyncOptions(reader); break;
                case RpcType.SyncInfectedWord: HandleSyncInfectedWord(reader); break;
                case RpcType.RpcPlayerUsedInfectedWord: HandleRpcPlayerUsedInfectedWord(reader); break;
                case RpcType.SyncMarkedPlayer: HandleSyncMarkedPlayer(reader); break;
                case RpcType.SetFakeTimeOfDeath: HandleSetFakeTimeOfDeath(reader); break;
                default: ModPlugin.Logger.LogWarning($"[RPC] Unhandled client message type: {rpcType}"); break;
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

        private void SendAck(uint messageId, int targetClientId)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.Acknowledge);
            writer.Write(messageId);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer, targetClientId);
            writer.Recycle();
        }

        private bool HasBeenReceived(uint messageId)
        {
            if (_receivedMessageIds.Contains(messageId)) return true;

            if (_receivedMessageIds.Count >= ReceivedMessageCacheSize)
            {
                _receivedMessageIds.Clear(); // Clear the set to prevent it from growing indefinitely
            }

            _receivedMessageIds.Add(messageId);
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
                 HandshakeManager.RemoveUnverifiedClient(senderId);
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

        private void HandleSetInfectedWord(MessageReader reader)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            var word = reader.ReadString();
            EchoManager.Instance.SetInfectedWord(word);

            // Broadcast the new word to all clients
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SyncInfectedWord);
            writer.Write(word);
            writer.EndMessage();
            Send(writer);
        }

        private void HandleSyncInfectedWord(MessageReader reader)
        {
            var word = reader.ReadString();
            EchoManager.Instance.SetInfectedWord(word);
        }

        private void HandleCmdPlayerUsedInfectedWord(MessageReader reader, int senderId)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            var playerId = reader.ReadByte();
            var player = GameData.Instance.GetPlayerById(playerId)?.Object;
            if (player != null)
            {
                // Host validates and then broadcasts the shimmer effect to all clients
                var writer = MessageWriter.Get(SendOption.Reliable);
                writer.StartMessage((byte)RpcType.RpcPlayerUsedInfectedWord);
                writer.Write(playerId);
                writer.EndMessage();
                Send(writer); // Broadcast to all
            }
        }

        private void HandleRpcPlayerUsedInfectedWord(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var player = GameData.Instance.GetPlayerById(playerId)?.Object;
            if (player != null)
            {
                var shimmer = player.gameObject.GetComponent<EchoShimmer>() ?? player.gameObject.AddComponent<EchoShimmer>();
                shimmer.StartShimmer();
            }
        }

        private void HandleMarkPlayer(MessageReader reader)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            var playerId = reader.ReadByte();
            GeistManager.Instance.MarkPlayer(playerId);

            var fakeTime = GeistManager.Instance.GetTimeOfDeath(playerId);

            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SyncMarkedPlayer);
            writer.Write(playerId);
            writer.Write(fakeTime);
            writer.EndMessage();
            Send(writer);
        }

        private void HandleSyncMarkedPlayer(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var fakeTime = reader.ReadSingle();
            GeistManager.Instance.SetMarkedPlayer(playerId, fakeTime);
        }

        private void HandleSetFakeTimeOfDeath(MessageReader reader)
        {
            var netId = reader.ReadPackedUInt32();
            var fakeTime = reader.ReadSingle();

            var obj = AmongUsClient.Instance.FindNetObject(netId);
            if (obj != null)
            {
                var body = obj.GetComponent<DeadBody>();
                if (body != null)
                {
                    body.TimeOfDeath = fakeTime;
                }
            }
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
