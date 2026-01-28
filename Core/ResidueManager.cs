using System.Collections.Generic;

namespace MyCustomRolesMod.Core
{
    public class ResidueManager
    {
        private static ResidueManager _instance;
        public static ResidueManager Instance => _instance ??= new ResidueManager();

        // Key: victim's PlayerId, Value: time the residue should disappear
        private readonly Dictionary<byte, float> _activeResidues = new Dictionary<byte, float>();
        private readonly object _lock = new object();

        private ResidueManager() { }

        public void AddResidue(byte victimId, float duration)
        {
            lock (_lock)
            {
                _activeResidues[victimId] = UnityEngine.Time.time + ModPlugin.ModConfig.ResidueDuration.Value;
            }
        }

        public bool IsResidue(byte victimId)
        {
            lock (_lock)
            {
                return _activeResidues.ContainsKey(victimId);
            }
        }

        public void RemoveResidue(byte victimId)
        {
            lock (_lock)
            {
                _activeResidues.Remove(victimId);
            }
        }

        public void CheckResidues()
        {
            lock (_lock)
            {
                if (!AmongUsClient.Instance.AmHost) return;

                var now = UnityEngine.Time.time;
                var toRemove = new List<byte>();
                foreach (var residue in _activeResidues)
                {
                    if (now >= residue.Value)
                    {
                        toRemove.Add(residue.Key);
                    }
                }

                foreach (var victimId in toRemove)
                {
                    _activeResidues.Remove(victimId);
                    Networking.RpcManager.Instance.SendRpcSetResidueState(victimId, false);
                    Networking.RpcManager.Instance.SendRpcSpawnResidueBody(victimId);
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _activeResidues.Clear();
            }
        }
    }
}
