using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MyCustomRolesMod.Core
{
    public class DendrochronologistManager
    {
        private static DendrochronologistManager _instance;
        public static DendrochronologistManager Instance => _instance ??= new DendrochronologistManager();

        private readonly Dictionary<string, List<RoomVisit>> _roomHistory = new Dictionary<string, List<RoomVisit>>();
        private readonly Dictionary<byte, string> _lastPlayerRoom = new Dictionary<byte, string>();
        private readonly object _lock = new object();

        private DendrochronologistManager() { }

        public struct RoomVisit
        {
            public byte PlayerId;
            public float Timestamp;
            public Color Color;
        }

        public void RecordVisit(string roomName, byte playerId, Color color)
        {
            lock (_lock)
            {
                if (!_roomHistory.ContainsKey(roomName))
                {
                    _roomHistory[roomName] = new List<RoomVisit>();
                }

                var history = _roomHistory[roomName];

                // Only record if it's a new entry (last recorded player in this room is different)
                if (history.Count == 0 || history.Last().PlayerId != playerId)
                {
                    history.Add(new RoomVisit { PlayerId = playerId, Timestamp = Time.time, Color = color });

                    // Keep only the last 4 visits
                    if (history.Count > 4)
                    {
                        history.RemoveAt(0);
                    }
                }
            }
        }

        public List<RoomVisit> GetHistory(string roomName)
        {
            lock (_lock)
            {
                if (_roomHistory.TryGetValue(roomName, out var history))
                {
                    return new List<RoomVisit>(history);
                }
                return new List<RoomVisit>();
            }
        }

        private Minigame[] _cachedMinigames;

        public string GetCurrentRoom(PlayerControl player)
        {
            if (ShipStatus.Instance == null) return "Unknown";

            if (_cachedMinigames == null)
            {
                _cachedMinigames = ShipStatus.Instance.allMinigames.ToArray();
            }

            // Proxy room detection using nearest Minigame
            Minigame closest = null;
            float minDistance = float.MaxValue;

            foreach (var minigame in _cachedMinigames)
            {
                if (minigame == null) continue;
                float dist = Vector2.Distance(player.GetTruePosition(), minigame.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = minigame;
                }
            }

            if (closest != null && minDistance < 5.0f)
            {
                // Try to get a meaningful name from the minigame or task
                return closest.name.Replace("(Clone)", "").Trim();
            }

            return "Hallway";
        }

        public void UpdateTracking()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player == null || player.Data == null || player.Data.IsDead) continue;

                string currentRoom = GetCurrentRoom(player);

                _lastPlayerRoom.TryGetValue(player.PlayerId, out string lastRoom);

                if (currentRoom != lastRoom)
                {
                    RecordVisit(currentRoom, player.PlayerId, Palette.PlayerColors[player.Data.ColorId]);
                    _lastPlayerRoom[player.PlayerId] = currentRoom;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _roomHistory.Clear();
                _lastPlayerRoom.Clear();
                _cachedMinigames = null;
            }
        }
    }
}
