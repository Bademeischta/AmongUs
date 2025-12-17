using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class PuppeteerManager
    {
        private static PuppeteerManager _instance;
        public static PuppeteerManager Instance => _instance ??= new PuppeteerManager();

        private readonly Dictionary<byte, string> _forcedMessages = new Dictionary<byte, string>();
        private readonly object _stateLock = new object();

        private PuppeteerManager() { }

        public void SetForcedMessage(byte playerId, string message)
        {
            lock (_stateLock)
            {
                if (string.IsNullOrEmpty(message))
                {
                    _forcedMessages.Remove(playerId);
                }
                else
                {
                    _forcedMessages[playerId] = message;
                }
            }
        }

        public string GetForcedMessage(byte playerId)
        {
            lock (_stateLock)
            {
                if (_forcedMessages.TryGetValue(playerId, out var message))
                {
                    _forcedMessages.Remove(playerId);
                    return message;
                }
                return null;
            }
        }

        public void ClearAllForcedMessages()
        {
            lock (_stateLock)
            {
                _forcedMessages.Clear();
            }
        }
    }
}
