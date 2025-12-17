using System.Collections.Generic;
using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class GeistManager
    {
        private static GeistManager _instance;
        public static GeistManager Instance => _instance ??= new GeistManager();

        private readonly Dictionary<byte, float> _markedPlayers = new Dictionary<byte, float>();
        private readonly object _lock = new object();
        public PlayerControl GeistPlayer { get; set; }

        private GeistManager() { }

        public void MarkPlayer(byte playerId)
        {
            lock (_lock)
            {
                if (_markedPlayers.ContainsKey(playerId)) return;
                _markedPlayers[playerId] = Time.time;
            }
        }

        public void SetMarkedPlayer(byte playerId, float time)
        {
            lock (_lock)
            {
                _markedPlayers[playerId] = time;
            }
        }

        public bool IsMarked(byte playerId)
        {
            lock (_lock)
            {
                return _markedPlayers.ContainsKey(playerId);
            }
        }

        public float GetTimeOfDeath(byte playerId)
        {
            lock (_lock)
            {
                _markedPlayers.TryGetValue(playerId, out var time);
                return time;
            }
        }

        public void RemoveMark(byte playerId)
        {
            lock (_lock)
            {
                _markedPlayers.Remove(playerId);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _markedPlayers.Clear();
                GeistPlayer = null;
            }
        }

        public Dictionary<byte, float> GetAllMarkedPlayers()
        {
            lock (_lock)
            {
                return new Dictionary<byte, float>(_markedPlayers);
            }
        }

        public void UpdateMarkedPlayers(float currentTime, System.Action<byte> onKill)
        {
            lock (_lock)
            {
                var toRemove = new List<byte>();
                foreach (var kvp in _markedPlayers)
                {
                    if (currentTime - kvp.Value > 45f) // MARKED_DEATH_TIMER
                    {
                        toRemove.Add(kvp.Key);
                        onKill?.Invoke(kvp.Key);
                    }
                }
                foreach (var id in toRemove)
                {
                    _markedPlayers.Remove(id);
                }
            }
        }
    }
}
