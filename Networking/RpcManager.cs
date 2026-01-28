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
        private readonly HashSet<uint> _receivedMessageIds = new HashSet<uint>();

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

        public void SendSetWitnessTestimony(byte playerId, string testimony)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SetWitnessTestimony);
            writer.Write(playerId);
            writer.Write(testimony);
            writer.EndMessage();
            Send(writer);
        }

        public void SendSetPuppeteerForcedMessage(byte playerId, string message)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SetPuppeteerForcedMessage);
            writer.Write(playerId);
            writer.Write(message ?? "");
            writer.EndMessage();
            Send(writer);
        }

        public void SendSetGlitchCorruptedSystem(int systemId)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SetGlitchCorruptedSystem);
            writer.Write(systemId);
            writer.EndMessage();
            Send(writer);
        }

        public void SendSetAnchorLinkedPlayers(byte player1, byte player2)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.SetAnchorLinkedPlayers);
            writer.Write(player1);
            writer.Write(player2);
            writer.EndMessage();
            Send(writer);
        }

        public void SendRpcRevealAnchorLink()
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.RpcRevealAnchorLink);
            writer.EndMessage();
            Send(writer);
        }

        public void SendCmdResidueKill(byte victimId)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.CmdResidueKill);
            writer.Write(victimId);
            writer.EndMessage();
            Send(writer);
        }

        public void SendRpcSpawnResidueBody(byte victimId)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.RpcSpawnResidueBody);
            writer.Write(victimId);
            writer.EndMessage();
            Send(writer);
        }

        public void SendRpcSetResidueState(byte victimId, bool isResidue)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.RpcSetResidueState);
            writer.Write(victimId);
            writer.Write(isResidue);
            writer.EndMessage();
            Send(writer);
        }

        public void SendRpcSetResidueState(byte victimId, bool isResidue)
        {
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.RpcSetResidueState);
            writer.Write(victimId);
            writer.Write(isResidue);
            writer.EndMessage();
            Send(writer);
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
                    case RpcType.SetInfectedWord: HandleSetInfectedWord(payloadReader); break;
                    case RpcType.SyncInfectedWord: HandleSyncInfectedWord(payloadReader); break;
                    case RpcType.CmdPlayerUsedInfectedWord: HandleCmdPlayerUsedInfectedWord(payloadReader, senderId); break;
                    case RpcType.RpcPlayerUsedInfectedWord: HandleRpcPlayerUsedInfectedWord(payloadReader); break;
                    case RpcType.MarkPlayer: HandleMarkPlayer(payloadReader); break;
                    case RpcType.SyncMarkedPlayer: HandleSyncMarkedPlayer(payloadReader); break;
                    case RpcType.SetFakeTimeOfDeath: HandleSetFakeTimeOfDeath(payloadReader); break;
                    case RpcType.SetWitnessTestimony: HandleSetWitnessTestimony(payloadReader); break;
                    case RpcType.SetPuppeteerForcedMessage: HandleSetPuppeteerForcedMessage(payloadReader); break;
                    case RpcType.SetGlitchCorruptedSystem: HandleSetGlitchCorruptedSystem(payloadReader); break;
                    case RpcType.SetAnchorLinkedPlayers: HandleSetAnchorLinkedPlayers(payloadReader); break;
                    case RpcType.RpcRevealAnchorLink: HandleRpcRevealAnchorLink(payloadReader); break;
                    case RpcType.CmdResidueKill: HandleCmdResidueKill(payloadReader, senderId); break;
                    case RpcType.RpcSpawnResidueBody: HandleRpcSpawnResidueBody(payloadReader); break;
                    case RpcType.RpcSetResidueState: HandleRpcSetResidueState(payloadReader); break;
                    case RpcType.RpcSetResidueState: HandleRpcSetResidueState(payloadReader); break;
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
            if (_receivedMessageIds.Count > 200)
            {
                _receivedMessageIds.Clear();
            }
            return !_receivedMessageIds.Add(messageId);
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
                lock(HandshakeManager._lock)
                {
                    HandshakeManager.UnverifiedClients.Remove(senderId);
                }
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
            // Clients receive the word from the host and update their local state
            if (AmongUsClient.Instance.AmHost) return;
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
            if (AmongUsClient.Instance.AmHost) return;
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

        private void HandleSetWitnessTestimony(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var testimony = reader.ReadString();
            WitnessManager.Instance.SetTestimony(playerId, testimony);
        }

        private void HandleSetPuppeteerForcedMessage(MessageReader reader)
        {
            var playerId = reader.ReadByte();
            var message = reader.ReadString();
            PuppeteerManager.Instance.SetForcedMessage(playerId, message);
        }

        private void HandleSetGlitchCorruptedSystem(MessageReader reader)
        {
            var systemId = reader.ReadInt32();
            GlitchManager.Instance.CorruptSystem(systemId);
        }

        private void HandleSetAnchorLinkedPlayers(MessageReader reader)
        {
            var player1 = reader.ReadByte();
            var player2 = reader.ReadByte();
            AnchorManager.Instance.SetLinkedPlayers(player1, player2);
        }

        private void HandleRpcRevealAnchorLink(MessageReader reader)
        {
            AnchorManager.Instance.RevealLink();
        }

        private void HandleCmdResidueKill(MessageReader reader, int senderId)
        {
            if (!AmongUsClient.Instance.AmHost) return;

            var victimId = reader.ReadByte();
            ResidueManager.Instance.AddResidue(victimId, ModPlugin.ModConfig.ResidueDuration.Value);

            SendRpcSetResidueState(victimId, true);
        }

        private void HandleRpcSpawnResidueBody(MessageReader reader)
        {
            var victimId = reader.ReadByte();
            var player = GameData.Instance.GetPlayerById(victimId)?.Object;
            if (player != null && player.Data.IsDead)
            {
                var body = Object.Instantiate(ShipStatus.Instance.KillBodyPrefab);
                body.ParentId = victimId;
                body.transform.position = player.transform.position;
            }
        }

        private void HandleRpcSetResidueState(MessageReader reader)
        {
            var victimId = reader.ReadByte();
            var isResidue = reader.ReadBoolean();
            if (isResidue)
            {
                ResidueManager.Instance.AddResidue(victimId, ModPlugin.ModConfig.ResidueDuration.Value);
            }
            else
            {
                ResidueManager.Instance.RemoveResidue(victimId);
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
