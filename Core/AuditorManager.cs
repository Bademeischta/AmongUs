using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class AuditorManager
    {
        private static AuditorManager _instance;
        public static AuditorManager Instance => _instance ??= new AuditorManager();

        private byte _auditedPlayerId = byte.MaxValue;
        private PlayerStateSnapshot _snapshot;

        private readonly object _stateLock = new object();

        private AuditorManager() { }

        public void SetAuditedPlayer(PlayerControl player)
        {
            if (player == null) return;

            lock (_stateLock)
            {
                _auditedPlayerId = player.PlayerId;
                _snapshot = new PlayerStateSnapshot(player);
            }
        }

        public PlayerStateSnapshot GetSnapshot()
        {
            lock (_stateLock)
            {
                return _snapshot;
            }
        }

        public byte GetAuditedPlayerId()
        {
            lock (_stateLock)
            {
                return _auditedPlayerId;
            }
        }

        public void Clear()
        {
            lock (_stateLock)
            {
                _auditedPlayerId = byte.MaxValue;
                _snapshot = null;
            }
        }
    }

    public class PlayerStateSnapshot
    {
        public Vector2 Position { get; }
        public int TasksCompleted { get; }
        public Color Color { get; }
        public string Name { get; }

        public PlayerStateSnapshot(PlayerControl player)
        {
            Position = player.GetTruePosition();
            TasksCompleted = GameData.Instance.GetPlayerById(player.PlayerId).Tasks.Count(t => t.IsComplete);
            Color = Palette.PlayerColors[player.Data.ColorId];
            Name = player.Data.PlayerName;
        }
    }
}
