using System.Collections.Generic;
using System.Linq;
using Hazel;
using MyCustomRolesMod.Networking.Packets;
using UnityEngine;

namespace MyCustomRolesMod.Networking
{
    public class RpcManager : MonoBehaviour
    {
        private static RpcManager _instance;
        public static RpcManager Instance => _instance;

        private readonly Dictionary<uint, PendingRpc> _pendingRpcs = new Dictionary<uint, PendingRpc>();
        private uint _nextMessageId = 0;

        void Awake() => _instance = this;

        void Update()
        {
            if (!AmongUsClient.Instance.AmHost) return;
            HandleTimeouts();
        }

        private void HandleTimeouts()
        {
            var now = Time.time;
            foreach (var rpc in _pendingRpcs.Values.ToList())
            {
                if (now - rpc.Timestamp > ModPlugin.ModConfig.RpcTimeoutSeconds.Value)
                {
                    if (rpc.RetryCount < ModPlugin.ModConfig.MaxRpcRetries.Value)
                    {
                        rpc.RetryCount++;
                        rpc.Timestamp = now;
                        ModPlugin.Logger.LogWarning($"[RPC] Message {rpc.MessageId} to client {rpc.TargetClientId} timed out. Retrying ({rpc.RetryCount}/{ModPlugin.ModConfig.MaxRpcRetries.Value})...");
                        AmongUsClient.Instance.SendOrDisconnect(rpc.Writer, rpc.TargetClientId ?? -1);
                    }
                    else
                    {
                        ModPlugin.Logger.LogError($"[RPC] Message {rpc.MessageId} failed after {ModPlugin.ModConfig.MaxRpcRetries.Value} retries. Giving up.");
                        _pendingRpcs.Remove(rpc.MessageId);
                        rpc.Writer.Recycle(); // Recycle on final failure
                    }
                }
            }
        }

        public void Send(MessageWriter writer) => SendTo(writer, null);

        public void SendTo(MessageWriter writer, int? targetClientId)
        {
            if (!AmongUsClient.Instance.AmHost)
            {
                writer.Recycle(); // Recycle if not sent
                return;
            }

            var messageId = _nextMessageId++;
            var finalWriter = MessageWriter.Get(SendOption.Reliable);
            finalWriter.StartMessage((byte)writer.Tag);
            finalWriter.Write(messageId);
            finalWriter.WriteBytesAndSize(writer.Buffer, writer.Length);
            finalWriter.EndMessage();

            writer.Recycle(); // Recycle the original writer immediately after copying

            _pendingRpcs[messageId] = new PendingRpc { MessageId = messageId, Timestamp = Time.time, Writer = finalWriter, TargetClientId = targetClientId };
            AmongUsClient.Instance.SendOrDisconnect(finalWriter, targetClientId ?? -1);
        }

        public void HandleAck(uint messageId)
        {
            if (_pendingRpcs.TryGetValue(messageId, out var rpc))
            {
                _pendingRpcs.Remove(messageId);
                rpc.Writer.Recycle(); // Recycle on successful ACK
                if (ModPlugin.ModConfig.IsDebug.Value)
                    ModPlugin.Logger.LogInfo($"[RPC] Received ACK for message {messageId}.");
            }
        }

        public static void SendAck(uint messageId)
        {
            if (AmongUsClient.Instance.AmHost) return;
            var writer = MessageWriter.Get(SendOption.Reliable);
            writer.StartMessage((byte)RpcType.Acknowledge);
            writer.Write(messageId);
            writer.EndMessage();
            AmongUsClient.Instance.SendOrDisconnect(writer);
            writer.Recycle(); // Recycle after sending
        }

        private class PendingRpc
        {
            public uint MessageId;
            public float Timestamp;
            public int RetryCount;
            public MessageWriter Writer;
            public int? TargetClientId;
        }
    }
}
