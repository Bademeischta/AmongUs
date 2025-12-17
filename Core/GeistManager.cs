using System.Collections.Generic;
using UnityEngine;

namespace MyCustomRolesMod.Core
{
    public class GeistManager
    {
        private static GeistManager _instance;
        public static GeistManager Instance => _instance ??= new GeistManager();

        // Key: PlayerId, Value: Time of Mark
        private readonly Dictionary<byte, float> _markedPlayers = new Dictionary<byte, float>();
        public PlayerControl GeistPlayer { get; set; }

        private GeistManager() { }

        public void MarkPlayer(byte playerId)
        {
            if (_markedPlayers.ContainsKey(playerId)) return;
            _markedPlayers[playerId] = Time.time;
        }

        public void SetMarkedPlayer(byte playerId, float time)
        {
            _markedPlayers[playerId] = time;
        }

        public bool IsMarked(byte playerId)
        {
            return _markedPlayers.ContainsKey(playerId);
        }

        public float GetTimeOfDeath(byte playerId)
        {
            _markedPlayers.TryGetValue(playerId, out var time);
            return time;
        }

        public void RemoveMark(byte playerId)
        {
            _markedPlayers.Remove(playerId);
        }

        public void Clear()
        {
            _markedPlayers.Clear();
            GeistPlayer = null;
        }

        public Dictionary<byte, float> GetAllMarkedPlayers()
        {
            return new Dictionary<byte, float>(_markedPlayers);
        }
    }
}
